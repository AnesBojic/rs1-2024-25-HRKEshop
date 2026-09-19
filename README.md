# 🛍️ HRKEshop - E-Commerce Platform

### HRKEshop is a full-stack e-commerce platform developed as part of the **RS1 (Software Solutions Development 1)** course for the academic year **2024/25**.  
The project demonstrates an online shopping system built with:

- ⚙️ **.NET C#**
- 🌐 **Angular (TypeScript)**
- 🗄️ **Microsoft SQL Server**

## Features:

### 👤 User Authentication 

 Secure login and registration system

### 🏢Multitenancy suport 

Multiple different companies can use the same application without interfiering with each other.


### 📦 Product Management 

Browse, search, create, delete and moddify products

### 🔍 Search Functionality 

 Find products quickly and efficiently

### 🗺️Leaflet maps

View and interact with map components inside the application.

### 🤖AI Chatbot assistant

Integrated chatbot for user support and product guidance.

### 🌍Multilanguage options

Switch between available languages in the UI.



## 🐳 Run with Docker (recommended for local testing)

Starts SQL Server, the .NET API (`:7000`), and the Angular frontend (`:4200`) together.

### Requirements

- [Docker](https://docs.docker.com/get-docker/) and Docker Compose

### Start everything

```bash
docker compose up --build
```

Then open:

- Frontend: http://localhost:4200
- API Swagger: http://localhost:7000/swagger
- Products page: http://localhost:4200/products

On first start the API applies EF migrations and a one-shot `seed` service calls `POST /data-seed-base`.

Optional chatbot AI key:

```bash
OPENAI_API_KEY=sk-... docker compose up --build
```

Stop and remove containers (keeps the SQL volume):

```bash
docker compose down
```

Reset the database volume:

```bash
docker compose down -v
```

## ⚙️ Technologies & Requirements

### Before running the application, install the following:

#### 🧰[.NET SDK 8.0+](https://dotnet.microsoft.com/en-us/download)

#### 🟩[Node.js 18+](https://nodejs.org/en) 

#### 🅰️[Angular CLI](https://v17.angular.io/cli)

#### 🗄️[SQL Server Managment studio & SQL Server](https://www.microsoft.com/en-us/sql-server)

## 🔧 Backend – Running the API Server

### 1. Setting up the database 

1. Update the database
```

update-database

```
2. Running the seed endpoint:

   It is required to run the seed endpoint provided bellow to give the database test records.

   In case it shows unAuthorized error (IGNORE IT), it works fine it just show error because extra security checks for when application is fully running.

```

 POST -> /data-seed-base

```

3. Backend app port (allready setted up)

```

 http://localhost:7000 

 ```

## 💻 Frontend – Running the Angular Application

### 1. Install Dependencies
```
cd frontend

or

npm install

```

### 2. Start the Application

```

npm start

 or

npm run dev

```

### 3. Frontend Base URL (already configured)

```

"http://localhost:4200"

```

### 🔐 Test login credentials

| Email               | Password |
| --------------------| -------- |
| any email in SSMS   | test     |
| ____________________| ________ |



Using the SQL Server Managment studio it will be required to take from the __*database*__ **HRKEshop** -> __*table*__ **AppUsers**.

Any email, and all of their passwords are "test".

In order to test the full scope of the application take user whose role_id = 1( Admin ), in that way it is possible to test all the functionalities of the application.

After loging in to test the application it is currently required to go to the following route, http://localhost:4200/products


## 📄 Autors:

### ✍️ Created by:
 * Anes Bojić
 * Džan Topalbegović

#### 🎓 Study Program: Razvoj softvera – 3rd year

#### 📘 Subject: Razvoj softvera 1 (RS1)