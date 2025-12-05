# NRL-Oppgave – Aviation Obstacle Reporting & Heatmap System

A complete ASP.NET Core MVC application for reporting, managing, and visualizing aviation obstacles, based on official datasets from **Nasjonalt register over luftfartshindre (NRL)**.

This system was developed as part of a coursework project and includes:

- Obstacle reporting by authenticated users  
- Administrative approval workflow  
- A GIS-based heatmap for pilots with live GPS tracking  
- Role-based access control  
- Dockerized MariaDB database  
- Conversion workflow from GML → GeoJSON (QGIS)

---

## ✨ Core Features

### 📝 Obstacle Reporting
Users can submit reports of aviation obstacles via a structured form.  
Features include:

- Registration form for obstacle data  
- Overview of submitted data before saving  
- Storage using EF Core and MariaDB  
- Role: **Registerfører**

### 🛠 Admin Panel
Admins can:

- View all obstacle submissions  
- Approve or reject reports  
- Manage data quality  
- See the full database overview  
- Role: **Admin**

### ✈️ Pilot View – Real-time Heatmap
The pilot-facing section includes:

- Heatmap based on national NRL dataset  
- Automatic weighting based on obstacle height  
- Live GPS tracking using browser geolocation  
- iOS/Android-style moving position marker  
- Smooth motion animation  
- Continuous calculation of obstacles within 5 km radius  
- Minimalist UI suitable for in-flight use  
- Role: **Pilot**

---

## 🗺 GIS & Dataset Workflow

The heatmap uses real GIS data.

### Data sources:
- Kartverket / **NRL aviation obstacle dataset** (GML format)

### Conversion steps:
1. Download NRL dataset from Kartverket  
2. Open GML in **QGIS**  
3. Convert to **WGS84 / GeoJSON**  
4. Export as `nrl_punkt.geojson`  
5. Place the file in: wwwroot/data/nrl_punkt.geojson

### Application flow:
- The backend exposes the dataset via `/Home/NrlPunktData`
- The frontend loads it dynamically into Leaflet
- Heatmap is generated with **Leaflet.heat**

---

## 🧱 Technologies Used

### Backend
- ASP.NET Core MVC (C#)
- EF Core (Pomelo provider)
- ASP.NET Identity (Roles: Admin, Pilot, Registerfører)
- Dependency Injection

### Frontend
- Leaflet.js  
- Leaflet.heat  
- Tailwind CSS utilities  
- Custom JavaScript for:
  - Geolocation
  - Smooth position animation
  - Heading-based rotation (optional)
  - Proximity calculation (Haversine)

### Database
- MariaDB (Dockerized)
- EF Core migrations
- Automatic role seeding

docker exec -it nrl-oppgave-db mariadb -uroot -prootpass obstaclesdb

CREATE TABLE IF NOT EXISTS obstacles (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    ObstacleName VARCHAR(100) NOT NULL,
    ObstacleHeight DOUBLE NOT NULL CHECK (ObstacleHeight >= 0),
    ObstacleDescription TEXT DEFAULT NULL,
    GeometryGeoJson LONGTEXT NULL,
    Latitude DOUBLE NULL,
    Longitude DOUBLE NULL,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    Status VARCHAR(20) NOT NULL DEFAULT 'Pending',
    SubmittedByUserId VARCHAR(255) NULL
);

### DevOps / Infrastructure
- Docker & Docker Compose
- Multi-container setup for:
  - Web application
  - MariaDB
  - phpMyAdmin (optional)

---

## 🐳 Docker Setup

The project includes `docker-compose.yml`.

Start everything: docker-compose up --build

Services that start:

| Service | Description |
|---------|-------------|
| web | ASP.NET Core MVC app |
| db  | MariaDB with mounted volume |
| phpmyadmin | GUI for DB management (if enabled) |

Default access: http://localhost:8080

---

## 🔐 Authentication & Roles

The system seeds these roles on startup:

- **Admin** – Can edit roles of users
  Admin user: admin@example.com 
  Password: AdminPassw0rd!
- **Registerfører** – Full report overview and ability to edit them
  Registerfører user: registerforer@example.com
  Password: RegisterPassw0rd!
- **Pilot** – Reports obstacles and heatmap overview

Identity uses:

- Email + password login  
- Adjustable password policy  
- Configurable via Program.cs  

---

## 🚀 Running Locally (without Docker)

### 1. Restore dependencies

### 2. Apply migrations

### 3. Run
dotnet run

Default URL: https://localhost:5001

---

## 🧪 Unit testing

A seperate xUnit test project has been created to test the "ObstacleData" domain model. The tests verify that the model correctly validates data according to the specified requirements. The test project is located in its own branch "feature/enhetstesting" The tests can be executed using:

dotnet test WebApplication2.sln

All tests passed succesfully, confirming that the domain logic behaves as expected.

---

## 🌐 Accessibility Testing (Wave)

To evaluate the accessibility of the application, we performed a WCAG-compliance check using WAVE browser extension. Wave reported minor improvements that could be made, but no critical accessibility barriers were detected.

---

## 🔮 Future Enhancements

Planned or suggested improvements:

- Toggle between heatmap / point map / filtered views  
- Filter by height (over 50 m / 100 m / 150 m)  
- Add “Threat Level” indicator (green/yellow/red)  
- Add planning mode vs. in-flight mode  
- Support for route visualization  
- UX improvements for mobile usage  
- Caching of GeoJSON for faster load  

---

## 📄 License

Based on publicly available data from Kartverket (NRL).  
This project is for academic and educational purposes.

---

## 👨‍✈️ Author

Developed by **Tobias**, UiA student, 2025.  
Includes GIS processing, web development, identity management, and data visualization.
  




