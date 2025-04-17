FROM postgres:16

# Install curl so we can download the init scripts
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Create the init directory and download both the schema and example data scripts
RUN mkdir -p /docker-entrypoint-initdb.d/ && \
    curl -fsSL "https://raw.githubusercontent.com/efficient-and-fun/operation/dev/plananaz/files/init_db.sql" \
         -o /docker-entrypoint-initdb.d/00-init_db.sql && \
    curl -fsSL "https://raw.githubusercontent.com/efficient-and-fun/operation/dev/plananaz/files/example_data.sql" \
         -o /docker-entrypoint-initdb.d/01-example_data.sql

# Expose the PostgreSQL port
EXPOSE 5432