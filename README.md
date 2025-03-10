# Keep It Tidy (KIT)


A simple app to keep your code tidy by reviewing it and generating reports.


---


## Table of Contents
1. [Overview](#overview)
2. [Installation](#installation)
3. [Technologies](#technologies)
4. [Core Features](#core-features)
5. [API](#api)
6. [Authentication](#authentication)
7. [Development Workflow](#development-workflow)
8. [Testing](#testing)
9. [Improvements & Features](#improvements--features)


---


## Overview

This project is a web application built using **Next.js** for the frontend and a **C# .NET backend** for code analysis. The application allows users to paste or write code in a text area and receive a detailed analysis report. The backend analyzes the code based on multiple aspects, including readability, security, and performance, and returns the results to the frontend.

## **Usage**
1. The user pastes their code into the text field and clicks "Analyze."
2. The backend process starts and analyzes the code.
3. The results are displayed as a report in the frontend, providing the user with insights into what can be improved in their code.

This project showcases how to combine frontend and backend technologies to build a functional and user-friendly application that solves a real-world problem.

---

## Installation


### Prerequisites
Clone this repository:
```bash
git clone https://github.com/timchristoffer/kit
```
Install dependencies:
```bash
npm install # For frontend (Next.js)
```
```bash
dotnet restore # For backend (C#)
```


#### Running the Project Locally
```bash
npm run dev
```
```bash
dotnet run
```
### Technologies
The project uses the following technologies:
 * **Frontend:** Next.js, React
 * **Backend:** C#, .NET Core WebAPI, .NET 9
 * **Database:** PostgreSQL (pgAdmin4)
 * **API Specification:** Swagger/OpenAPI
 * **Testing:** Jest (frontend), xUnit (backend)


### Core Features


#### File Upload
 * Users can upload code files for analysis.
    * Supported file types: ``.js``, ``.ts``, ``.jsx``, ``.tsx``, ``.cs``, etc.
     
#### Code Analysis
 * The uploaded file undergoes analysis based on the language.
    * **Javascript:** ESLint for code quality checks.
    * **C#:** Roslyn for preformance and security analysis.
    * **KIT Custom Analytics Tool:** KIT CAT for overall analysis.
     
#### PDF Report Generation
 * After analysis, users can download a PDF report with:
    * Code quality issues.
    * Suggestions for improvement.
    * Performance bottlenecks.
    * Security vulnerabilities.


### API

#### Analysis API

##### Analyze Code
**Endpoint:** `POST /api/analysis`
- **Description:** Analyzes a given code snippet or file and returns an analysis report.
- **Request Body:**
  ```json
  {
    "SourceType": "file" | "content",
    "FileId": "GUID (optional if SourceType is 'file')",
    "Content": "string (optional if SourceType is 'content')"
  }
  ```
- **Responses:**
  - `200 OK`: Returns an `AnalysisReport`.
  - `400 Bad Request`: If no code is provided.
  - `500 Internal Server Error`: If analysis fails.

#### Get Analysis Report
**Endpoint:** `GET /api/analysis/{id}`
- **Description:** Retrieves a previously generated analysis report by ID.
- **Responses:**
  - `200 OK`: Returns an `AnalysisReport`.
  - `404 Not Found`: If report does not exist.

#### Download Report
**Endpoint:** `GET /api/analysis/download/{id}`
- **Description:** Downloads the analysis report as a PDF.
- **Responses:**
  - `200 OK`: Returns the PDF file.
  - `404 Not Found`: If the report or PDF does not exist.

#### Code Snippet API

##### Create Code Snippet
**Endpoint:** `POST /api/codesnippets`
- **Description:** Creates a new code snippet.
- **Request Body:**
  ```json
  {
    "Content": "string"
  }
  ```
- **Responses:**
  - `201 Created`: Returns the created `CodeSnippet`.
  - `400 Bad Request`: If input is invalid.

#### Get Code Snippet
**Endpoint:** `GET /api/codesnippets/{id}`
- **Description:** Retrieves a code snippet by ID.
- **Responses:**
  - `200 OK`: Returns a `CodeSnippet`.
  - `404 Not Found`: If snippet does not exist.

#### File API

##### Upload File
**Endpoint:** `POST /api/files`
- **Description:** Uploads a source code file for analysis.
- **Request:** Form-data with a file.
- **Responses:**
  - `200 OK`: Returns file metadata.
  - `400 Bad Request`: If file is missing or has an invalid type.
  - `500 Internal Server Error`: If upload fails.

#### Get File by ID
**Endpoint:** `GET /api/files/{id}`
- **Description:** Retrieves a file by ID.
- **Responses:**
  - `200 OK`: Returns file metadata and content.
  - `404 Not Found`: If file does not exist.

#### User API

##### Create User
**Endpoint:** `POST /users`
- **Description:** Creates a new user.
- **Request Body:**
  ```json
  {
    "UserName": "string",
    "Email": "string"
  }
  ```
- **Responses:**
  - `201 Created`: Returns the created user.
  - `400 Bad Request`: If input is invalid.

#### Get User by ID
**Endpoint:** `GET /users/{id}`
- **Description:** Retrieves user details by ID.
- **Responses:**
  - `200 OK`: Returns the user.
  - `404 Not Found`: If user does not exist.

### Development Workflow
#### Local Development Flow
 * **Branching:** Use GitHub flow (feature branches for new features, merge to ``main`` when done).
 * **Pull Requests:** Create a pull request for each feature, including tests and documentation.
   
#### Deployment
Deployment occurs via Github Actions:
 * Backend is deployed on Azure.
 * Frontend is deployed via Vercel (Next.js)


### Testing
#### Frontend (Jest)
 * Unit tests are located in the ``frontend/__tests__`` folder.
 * Run tests with:
   ```bash
    npm test
    ```
   
#### Backend (xUnit)
 * Test the API with xUnit. Run tests via:
   ```bash
    dotnet test
    ```


### Improvements & Features
