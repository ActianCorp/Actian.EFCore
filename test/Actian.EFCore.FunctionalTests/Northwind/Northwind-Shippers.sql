-- set quoted_identifier on
-- \nocontinue\p\g

-- set identity_insert "Shippers" on
-- \nocontinue\p\g

-- ALTER TABLE "Shippers" NOCHECK CONSTRAINT ALL
-- \nocontinue\p\g

INSERT INTO "Shippers" ("ShipperID","CompanyName","Phone") VALUES(1,'Speedy Express','(503) 555-9831');
INSERT INTO "Shippers" ("ShipperID","CompanyName","Phone") VALUES(2,'United Package','(503) 555-3199');
INSERT INTO "Shippers" ("ShipperID","CompanyName","Phone") VALUES(3,'Federal Shipping','(503) 555-9931');
\nocontinue\p\g

-- set identity_insert "Shippers" off
-- \nocontinue\p\g

-- ALTER TABLE "Shippers" CHECK CONSTRAINT ALL
-- \nocontinue\p\g
