Build started...
Build succeeded.
DELETE FROM Persons;
DELETE FROM Joinlists;
DELETE FROM Accessdoors;
CREATE TABLE "Accessdoors" (
    "ID" INTEGER NOT NULL CONSTRAINT "PK_Accessdoors" PRIMARY KEY AUTOINCREMENT,
    "StartDate" TEXT NULL,
    "SearchColumn" TEXT NULL,
    "Room" TEXT NULL,
    "Session" TEXT NULL,
    "AccessdoorID" INTEGER NULL,
    CONSTRAINT "FK_Accessdoors_Accessdoors_AccessdoorID" FOREIGN KEY ("AccessdoorID") REFERENCES "Accessdoors" ("ID")
);


CREATE TABLE "Article" (
    "ID" INTEGER NOT NULL CONSTRAINT "PK_Article" PRIMARY KEY AUTOINCREMENT,
    "Number" TEXT NULL,
    "ReleaseDate" TEXT NOT NULL,
    "Gender" TEXT NULL,
    "Count" TEXT NOT NULL,
    "Date" TEXT NOT NULL,
    "Time" TEXT NOT NULL,
    "Location" TEXT NULL
);


CREATE TABLE "Doorsystem" (
    "ID" INTEGER NOT NULL CONSTRAINT "PK_Doorsystem" PRIMARY KEY AUTOINCREMENT,
    "Date" TEXT NOT NULL,
    "Session" TEXT NULL,
    "SessionName" TEXT NULL,
    "Room" TEXT NULL,
    "TimeRange" TEXT NULL
);


CREATE TABLE "Joinlists" (
    "ID" INTEGER NOT NULL CONSTRAINT "PK_Joinlists" PRIMARY KEY AUTOINCREMENT,
    "StartDate" TEXT NOT NULL,
    "EndDate" TEXT NOT NULL,
    "RegNo" TEXT NULL,
    "FirstName" TEXT NULL,
    "LastName" TEXT NULL,
    "ChineseName" TEXT NULL,
    "Country" TEXT NULL,
    "RegistrationStatus" TEXT NULL,
    "JoinlistID" INTEGER NULL,
    CONSTRAINT "FK_Joinlists_Joinlists_JoinlistID" FOREIGN KEY ("JoinlistID") REFERENCES "Joinlists" ("ID")
);


CREATE TABLE "Login" (
    "ID" INTEGER NOT NULL CONSTRAINT "PK_Login" PRIMARY KEY AUTOINCREMENT,
    "Username" TEXT NULL,
    "Password" TEXT NULL
);


CREATE TABLE "Persons" (
    "ID" INTEGER NOT NULL CONSTRAINT "PK_Persons" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "Age" INTEGER NOT NULL,
    "Gender" TEXT NOT NULL
);


CREATE TABLE "Registration" (
    "ID" INTEGER NOT NULL CONSTRAINT "PK_Registration" PRIMARY KEY AUTOINCREMENT,
    "DisplayLocation" TEXT NULL,
    "DisplayStatus" TEXT NULL,
    "ItemName" TEXT NULL,
    "TotalAmount" INTEGER NULL,
    "TotalAmountUSD" INTEGER NULL
);


CREATE INDEX "IX_Accessdoors_AccessdoorID" ON "Accessdoors" ("AccessdoorID");


CREATE INDEX "IX_Joinlists_JoinlistID" ON "Joinlists" ("JoinlistID");



