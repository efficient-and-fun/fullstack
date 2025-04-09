# Project ReadMe

## Development Setup

### Frontend
Prerequsite
    -node version 20+ installed

1. Navigate to the frontend directory:
    ```bash
    cd frontend
    ```

2. Install dependencies:
    ```bash
    npm install
    ```

3. Start the development server:
    ```bash
    npm run dev
    ```

4. Run tests:
    ```bash
    npm test
    ```

### Backend
Prerequisite
    -dotnet 8.0 installed

1. Navigate to the backend/WebApi directory:
    ```bash
    cd backend/WebApi
    ```

2. Restore dependencies:
    ```bash
    dotnet restore 
    ```

3. Build the project:
    ```bash
    dotnet build
    ```

4. Run the project:
    ```bash
    dotnet run
    ```
5. Run tests:
    ```bash
    dotnet test
    ```
### Database
Prerequsiste:
    -pgadmin
1.  Open pg admin

2.  Create user plananaz
        right click on login/group/roles
        create -> Create login
        General
        Name: plananaz
        Privileges
        activate all

3.  Open query tool
        right click on database

4.  Insert and run init_db.sql script




### Dockerize

#### Build and Run Docker Images Locally

- Build Docker Images:
    - Frontend:
        ```bash
        docker build -t frontend:latest ./frontend
        ```
    - Backend:
        ```bash
        docker build -t backend:latest ./backend/WebApi
        ```

- Run Docker Containers:
    - Frontend:
        ```bash
        docker run -p 80:3000 frontend:latest
        ```
    - Backend:
        ```bash
        docker run -p 5246:5246 backend:latest
        ```


### Additional Commands
- Lint the codebase:
  ```bash
  npm run lint
  ```

- Build the project:
  ```bash
  npm run build
  ```