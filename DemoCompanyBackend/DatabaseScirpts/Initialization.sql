-- Create the database
CREATE DATABASE IF NOT EXISTS DemoCompanyDB;

-- Use the newly created database
USE DemoCompanyDB;

-- Create the Department table
CREATE TABLE IF NOT EXISTS Department (
    DepartmentId INT AUTO_INCREMENT PRIMARY KEY,
    DepartmentName VARCHAR(500) CHARACTER SET UTF8MB4
);

-- Create the Employee table
CREATE TABLE IF NOT EXISTS Employee (
    EmployeeId INT AUTO_INCREMENT PRIMARY KEY,
    EmployeeName VARCHAR(255) CHARACTER SET UTF8MB4,
    Department VARCHAR(255) CHARACTER SET UTF8MB4,
    DateOfJoining DATE,
    PhotoFileName VARCHAR(255) CHARACTER SET UTF8MB4
);
