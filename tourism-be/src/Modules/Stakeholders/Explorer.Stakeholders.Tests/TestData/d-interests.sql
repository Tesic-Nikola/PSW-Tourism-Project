-- Insert predefined interests
INSERT INTO stakeholders."Interests"("Id", "Name") VALUES (-1, 'priroda');
INSERT INTO stakeholders."Interests"("Id", "Name") VALUES (-2, 'umetnost');
INSERT INTO stakeholders."Interests"("Id", "Name") VALUES (-3, 'sport');
INSERT INTO stakeholders."Interests"("Id", "Name") VALUES (-4, 'soping');
INSERT INTO stakeholders."Interests"("Id", "Name") VALUES (-5, 'hrana');

-- Add some test interests for existing test users
INSERT INTO stakeholders."PersonInterests"("Id", "PersonId", "InterestId") VALUES (-1, -21, -1); -- turista1 likes priroda
INSERT INTO stakeholders."PersonInterests"("Id", "PersonId", "InterestId") VALUES (-2, -21, -3); -- turista1 likes sport
INSERT INTO stakeholders."PersonInterests"("Id", "PersonId", "InterestId") VALUES (-3, -22, -2); -- turista2 likes umetnost
INSERT INTO stakeholders."PersonInterests"("Id", "PersonId", "InterestId") VALUES (-4, -22, -5); -- turista2 likes hrana
INSERT INTO stakeholders."PersonInterests"("Id", "PersonId", "InterestId") VALUES (-5, -23, -4); -- turista3 likes soping