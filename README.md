# Système de Réservation de Salles

## Présentation

Ce projet est un système complet de réservation de salles composé d'une API backend en .NET et d'une interface utilisateur en Vue.js. Il permet aux utilisateurs de consulter les salles disponibles, de créer des réservations, et de gérer leurs événements.

## Architecture

Le projet est divisé en deux parties principales :

### Backend (RoomBookingApi)

- Développé avec .NET 8.0
- API REST pour la gestion des salles et des réservations
- Base de données SQLite pour le stockage des données
- Utilisation d'Entity Framework Core pour l'ORM

### Frontend (room-booking-app)

- Développé avec Vue.js 3 et Vite
- Interface utilisateur réactive et moderne
- Utilisation de Pinia pour la gestion d'état
- Composants réutilisables pour la gestion des salles et des réservations

## Fonctionnalités

- Affichage des salles disponibles
- Création, modification et suppression de réservations
- Vue calendrier hebdomadaire des réservations
- Validation des disponibilités des salles
- Envoi d'emails de confirmation (simulé)
- Export des réservations au format iCalendar
- Intégration possible avec Google Calendar et Outlook (préparé)

## Installation

### Prérequis

- .NET SDK 8.0 ou supérieur
- Node.js 16 ou supérieur
- npm ou yarn

### Backend

bash
cd RoomBookingApi
dotnet restore
dotnet run

### Frontend

bash
cd room-booking-app
npm install
npm run dev

# Configuration

### Backend

Les paramètres de connexion à la base de données peuvent être modifiés dans le fichier `appsettings.json`.

### Frontend

Les paramètres de l'API peuvent être modifiés dans le fichier `src/services/api.js`.

## Utilisation

1. Accédez à la page d'accueil
2. Naviguez vers "Salles à réserver"
3. Sélectionnez une date et une salle disponible
4. Remplissez le formulaire de réservation
5. Confirmez votre réservation


## Développement

### Structure du projet backend

RoomBookingApi/
├── Data/ # Contexte de base de données et migrations
├── Models/ # Modèles de données
├── Program.cs # Point d'entrée et configuration
└── appsettings.json # Configuration


### Structure du projet frontend

room-booking-app/
├── public/ # Ressources statiques
├── src/
│ ├── components/ # Composants Vue
│ ├── services/ # Services API et utilitaires
│ ├── store/ # Gestion d'état avec Pinia
│ ├── App.vue # Composant racine
│ └── main.js # Point d'entrée
└── index.html # Page HTML principale