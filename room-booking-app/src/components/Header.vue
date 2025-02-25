<template>
  <header class="header">
    <h1>Système de Réservation de Salles</h1>
    <nav class="nav">
      <router-link to="/" class="nav-link">Accueil</router-link>
      <router-link to="/rooms" class="nav-link">Salles à réserver</router-link>
      <button @click="toggleLogin" class="nav-link">
        {{ isLoggedIn ? 'Déconnexion' : 'Connexion' }}
      </button>
    </nav>
  </header>
</template>

<script>
import { ref } from 'vue';

export default {
  name: 'Header',
  setup() {
    const isLoggedIn = ref(false);

    const toggleLogin = () => {
      isLoggedIn.value = !isLoggedIn.value;
    };

    return {
      isLoggedIn,
      toggleLogin
    };
  }
}
</script>

<style scoped>
.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 15px 30px;
  background-color: #ffffff;
  color: #333;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
  border-bottom: 2px solid #f0f0f0;
  transition: background-color 0.3s, box-shadow 0.3s;
  font-family: 'Roboto', sans-serif;
}

.header:hover {
  background-color: #f9f9f9;
  box-shadow: 0 6px 12px rgba(0, 0, 0, 0.15);
}

.nav {
  display: flex;
  gap: 25px;
}

.nav-link {
  color: #333;
  text-decoration: none;
  font-weight: 500;
  padding: 8px 16px;
  border-radius: 5px;
  transition: background-color 0.3s, color 0.3s;
  font-family: 'Montserrat', sans-serif;
}

.nav-link:hover {
  background-color: #42b983;
  color: white;
}

.nav-link:active {
  background-color: #369f6b;
}
</style> 