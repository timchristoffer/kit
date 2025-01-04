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
Provide a brief description of what the project is about, its goals, and key features.


- Example: This project is a code review tool that allows users to upload code files or paste code directly for analysis. It generates a comprehensive report and outputs it as a downloadable PDF.


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
 * **Database:** SQL Server
 * **Authentication:** OAuth 2.0 (GitHub, Google, Microsoft)
 * **API Specification:** Swagger/OpenAPI
 * **Testing:** Jest (frontend), xUnit (backend)


### Core Features


#### File Upload
 * Users can upload code files for analysis.
    * Supported file types: ``.js``, ``.cs``, ``.py``, etc.
     
#### Code Analysis
 * The uploaded file undergoes analysis based on the language.
    * **Javascript:** ESLint for code quality checks.
    * **C#:** Roslyn for preformance and security analysis.
    * **Python:** PyLint for identifying potential issues.
     
#### PDF Report Generation
 * After analysis, users can download a PDF report with:
    * Code quality issues.
    * Suggestions for improvement.
    * Performance bottlenecks.
    * Security vulnerabilities.


### API


### Authentication


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
