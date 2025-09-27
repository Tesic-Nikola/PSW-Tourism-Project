using Explorer.BuildingBlocks.Core.Events;
using Explorer.Bookings.Core.Domain;
using Explorer.Bookings.Core.Domain.RepositoryInterfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using AutoMapper;
using Explorer.Bookings.API.Dtos;

namespace Explorer.Bookings.Core.UseCases.EventHandlers;

public class TourInfoResponseEventHandler : IEventHandler<TourInfoResponseEvent>
{
    private readonly IShoppingCartRepository _cartRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<TourInfoResponseEventHandler> _logger;
    private static readonly ConcurrentDictionary<Guid, PendingCartRequest> _pendingRequests = new();

    public TourInfoResponseEventHandler(IShoppingCartRepository cartRepository, IMapper mapper, ILogger<TourInfoResponseEventHandler> logger)
    {
        _cartRepository = cartRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public static void AddPendingRequest(Guid requestId, PendingCartRequest request)
    {
        _pendingRequests[requestId] = request;
    }

    public static bool RemovePendingRequest(Guid requestId, out PendingCartRequest request)
    {
        return _pendingRequests.TryRemove(requestId, out request);
    }

    public async Task HandleAsync(TourInfoResponseEvent @event)
    {
        _logger.LogInformation("Handling tour info response for RequestId: {RequestId}", @event.RequestId);

        if (!RemovePendingRequest(@event.RequestId, out var pendingRequest))
        {
            _logger.LogWarning("No pending request found for RequestId: {RequestId}", @event.RequestId);
            return;
        }

        try
        {
            if (!@event.IsSuccess)
            {
                pendingRequest.CompletionSource.SetException(new InvalidOperationException(@event.ErrorMessage));
                return;
            }

            // Get or create cart
            var cart = _cartRepository.GetByTouristId(@event.TouristId);
            if (cart == null)
            {
                cart = new ShoppingCart(@event.TouristId);
                cart = _cartRepository.Create(cart);
            }

            // Check if item already exists in cart
            var existingItem = cart.Items.FirstOrDefault(i => i.TourId == @event.TourId);
            if (existingItem != null)
            {
                // Remove existing and add new with updated quantity
                cart.Items.Remove(existingItem);
                var newItem = new CartItem(@event.TourId, @event.TourName, @event.TourPrice,
                    @event.TourStartDate, existingItem.Quantity + @event.Quantity);
                cart.Items.Add(newItem);
            }
            else
            {
                // Add new item
                var newItem = new CartItem(@event.TourId, @event.TourName, @event.TourPrice,
                    @event.TourStartDate, @event.Quantity);
                cart.Items.Add(newItem);
            }

            cart.MarkAsUpdated();
            cart = _cartRepository.Update(cart);

            var cartDto = _mapper.Map<ShoppingCartDto>(cart);
            pendingRequest.CompletionSource.SetResult(cartDto);

            _logger.LogInformation("Successfully added tour {TourId} to cart for tourist {TouristId}", @event.TourId, @event.TouristId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing tour info response for RequestId: {RequestId}", @event.RequestId);
            pendingRequest.CompletionSource.SetException(ex);
        }
    }
}