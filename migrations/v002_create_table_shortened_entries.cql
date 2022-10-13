CREATE TABLE IF NOT EXISTS url_shortener.shortened_entries (
    alias text PRIMARY KEY,
    url text,
    creation timestamp,
    expiration timestamp
);