CREATE TABLE IF NOT EXISTS url_shortener.shortened_entries (
    alias text PRIMARY KEY,
    url text,
    user_id uuid,
    creation timestamp,
    expiration timestamp
);