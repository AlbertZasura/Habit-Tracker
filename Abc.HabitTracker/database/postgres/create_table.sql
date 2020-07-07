CREATE TABLE "users"
(
    id         UUID PRIMARY KEY,
    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE "habits"
(
    id          UUID PRIMARY KEY,
    name       	VARCHAR(100) NOT NULL,
    days_off    VARCHAR[] NOT NULL,
    user_id    	UUID NOT NULL,
    created_at  TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    updated_at  TIMESTAMPTZ,
    deleted_at  TIMESTAMPTZ,
    FOREIGN KEY (user_id) REFERENCES "users" (id)
);

CREATE TABLE "badges"
(
    id         	UUID PRIMARY KEY,
    name       	VARCHAR(100) NOT NULL,
    description VARCHAR(100) NOT NULL,
    created_at 	TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE "logs"
(
    id         	UUID PRIMARY KEY,
    habit_id   	UUID NOT NULL,
	log			TIMESTAMP WITHOUT TIME ZONE NOT NULL,
    day			VARCHAR(10) NOT NULL,
    current_streak		INT NOT NULL,
    longest_streak		INT NOT NULL,
    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (habit_id) REFERENCES "habits" (id)
);

CREATE TABLE "achievement"
(
    user_id    UUID NOT NULL,
    badge_id    UUID NOT NULL,
    FOREIGN KEY (user_id) REFERENCES "users" (id),
    FOREIGN KEY (badge_id) REFERENCES "badges" (id),
    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE "log_snapshot"
(
    id         	UUID PRIMARY KEY,
    habit_id   	UUID NOT NULL,
	log			TIMESTAMP WITHOUT TIME ZONE[] NOT NULL,
    day			VARCHAR[] NOT NULL,
    current_streak		INT NOT NULL,
    longest_streak		INT NOT NULL,
	last_log_id			UUID NOT NULL,
	last_log_created_at	TIMESTAMPTZ NOT NULL,
    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (habit_id) REFERENCES "habits" (id),
	FOREIGN KEY (last_log_id) REFERENCES "logs" (id)
);

INSERT INTO "users" (id) VALUES('4fbb54f1-f340-441e-9e57-892329464d56');
