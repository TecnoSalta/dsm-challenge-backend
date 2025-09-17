INSERT INTO public."Services" ("Id", "Date", "DurationDays", "CarId") VALUES
(gen_random_uuid(), CURRENT_DATE + INTERVAL '9 day', 2, '11111111-1111-1111-1111-111111111111'),
(gen_random_uuid(), CURRENT_DATE + INTERVAL '11 day', 2, '11111111-1111-1111-1111-111111111112'),
(gen_random_uuid(), CURRENT_DATE + INTERVAL '13 day', 2, '11111111-1111-1111-1111-111111111113');