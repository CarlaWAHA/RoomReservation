import React from 'react';
import { Link } from 'react-router-dom';
import styles from './HomePage.module.css';

const HomePage = () => {
    return (
        <div>
            <h1>Bienvenue sur la page d'accueil</h1>
            <p>Ceci est la page d'accueil de votre application.</p>
            <Link to="/rooms" className={styles.btnLink}>Voir les salles disponibles</Link>
        </div>
    );
};

export default HomePage; 