# CompanyAPI

## Overview

This project is a backend API for managing company departments and employees. It is developed using ASP.NET and provides a CRUD (Create, Read, Update, Delete) API for managing department and employee records.

## Technologies Used

- **ASP.NET**: The primary framework used for building the backend API.
- **C#**: The main programming language used for developing the application.
- **SQL**: Used for database operations. The project uses raw SQL queries for data operations, providing a deeper understanding of database connections and queries.
- **MySQL**: The database used for storing department and employee records.

## Features

### Department API

The Department API allows you to manage departments within an organization. It provides endpoints for retrieving, creating, updating, and deleting department records in the database. The API is designed to perform basic CRUD operations on department records in the database.

### Employee API

The Employee API is responsible for managing employee data. It interacts with the MySQL database to perform CRUD operations on employee records. The controller follows best practices for handling database connections and uses parameterized queries to prevent SQL injection.

## Why Raw SQL Instead of EF Core?

In this project, raw SQL queries were preferred over Entity Framework (EF) Core. The primary reason for this decision was to gain a deeper understanding of how database connections and queries work. Using raw SQL provides a hands-on experience with crafting queries, managing database connections, and understanding the intricacies of data operations.
