CREATE TABLE IF NOT EXISTS "Users" (
    "Id" uuid NOT NULL DEFAULT gen_random_uuid(),
    "Email" text NOT NULL,
    "PasswordHash" text NOT NULL,
    "FirstName" text,
    "LastName" text,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_Users" PRIMARY KEY ("Id")
);

CREATE TABLE IF NOT EXISTS "Prospects" (
    "Id" uuid NOT NULL DEFAULT gen_random_uuid(),
    "PlayerName" text NOT NULL,
    "Team" text NOT NULL,
    "Position" text NOT NULL,
    "Age" integer NOT NULL,
    "ETA" text,
    "Rank" integer NOT NULL,
    "Source" text NOT NULL,
    CONSTRAINT "PK_Prospects" PRIMARY KEY ("Id")
);

CREATE TABLE IF NOT EXISTS "Rankings" (
    "Id" uuid NOT NULL DEFAULT gen_random_uuid(),
    "Rank" integer NOT NULL,
    "PlayerName" text NOT NULL,
    "Team" text NOT NULL,
    "Position" text NOT NULL,
    "Age" integer NOT NULL,
    "ETA" text,
    "Score" integer NOT NULL,
    "Volatility" text NOT NULL,
    "Consensus" integer NOT NULL,
    CONSTRAINT "PK_Rankings" PRIMARY KEY ("Id")
);

CREATE UNIQUE INDEX IF NOT EXISTS "IX_Users_Email" ON "Users" ("Email");
