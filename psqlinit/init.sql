ALTER ROLE postgres WITH SUPERUSER;

CREATE DATABASE "footballstats";

\c "footballstats"

CREATE TABLE users (
	username text PRIMARY KEY,
	"password" text NULL,
	vars jsonb NULL,
	createdate timestamp with time zone,
	modifydate timestamp with time zone
);

CREATE TABLE teams (
	id serial PRIMARY KEY,
	"name" text NOT NULL UNIQUE,
	players _text NULL,
	createdate timestamp with time zone,
	modifydate timestamp with time zone
);
CREATE INDEX teams_name_index ON teams USING btree ("name");

CREATE TABLE games (
	id serial PRIMARY KEY,
	team1 integer NOT NULL,
	team2 integer NOT NULL,
	goals1 integer DEFAULT 0,
	goals2 integer DEFAULT 0,
	"status" text, -- a status like Playing, Completed, Archived (if we don't want to use it the global stats f.e.)
	vars jsonb NULL, -- we can use this field to store any additional data as the game stats f.e.
	createdate timestamp with time zone,
	completeddate timestamp with time zone, -- needs the game date for the historical games log
	modifydate timestamp with time zone
);
CREATE INDEX games_completeddate_index ON games USING btree (completeddate);