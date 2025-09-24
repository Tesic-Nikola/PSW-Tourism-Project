-- Insert predefined interests
INSERT INTO stakeholders."Interests"("Id", "Name") VALUES (-1, 'Nature');
INSERT INTO stakeholders."Interests"("Id", "Name") VALUES (-2, 'Art');
INSERT INTO stakeholders."Interests"("Id", "Name") VALUES (-3, 'Sport');
INSERT INTO stakeholders."Interests"("Id", "Name") VALUES (-4, 'Shopping');
INSERT INTO stakeholders."Interests"("Id", "Name") VALUES (-5, 'Food');

-- Add some test interests for existing test users
INSERT INTO stakeholders."PersonInterests"("Id", "PersonId", "InterestId") VALUES (-1, -21, -1); -- turista1 likes Nature
INSERT INTO stakeholders."PersonInterests"("Id", "PersonId", "InterestId") VALUES (-2, -21, -3); -- turista1 likes Sport
INSERT INTO stakeholders."PersonInterests"("Id", "PersonId", "InterestId") VALUES (-3, -22, -2); -- turista2 likes Art
INSERT INTO stakeholders."PersonInterests"("Id", "PersonId", "InterestId") VALUES (-4, -22, -5); -- turista2 likes Food
INSERT INTO stakeholders."PersonInterests"("Id", "PersonId", "InterestId") VALUES (-5, -23, -4); -- turista3 likes Shopping