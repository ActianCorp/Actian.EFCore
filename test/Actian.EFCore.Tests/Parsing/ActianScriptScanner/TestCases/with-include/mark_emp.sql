CREATE PROCEDURE mark_emp
    (id INTEGER NOT NULL, label VARCHAR(100)) AS
BEGIN
    UPDATE employee
        SET COMMENT = :label
        WHERE id = :id;

    IF iirowcount = 1 THEN
        MESSAGE 'Employee was marked';
        COMMIT;
        RETURN 1;
    ELSE
        MESSAGE 'Employee was not marked - record error';
        ROLLBACK;
        RETURN 0;
    ENDIF;
END;
\nocontinue\p\g