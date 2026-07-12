DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'mysolution') THEN
CREATE SCHEMA mysolution;
END IF;
END $EF$;