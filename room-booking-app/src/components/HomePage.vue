<template>
  <div class="home-page">
    <h1>Bienvenue sur la page d'accueil</h1>
    <p>Ceci est la page d'accueil de votre application.</p>
    <router-link to="/rooms" class="btn-link">Voir les salles disponibles</router-link>
  </div>
</template>

<script>
export default {
  name: 'HomePage'
}
</script>

<style scoped>
.home-page {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 40px;
  background-color: #f5f5f5;
  min-height: 100vh;
  text-align: center;
  font-family: 'Roboto', sans-serif;
}

h1 {
  font-size: 2.5em;
  color: #333;
  margin-bottom: 20px;
  font-family: 'Montserrat', sans-serif;
}

p {
  font-size: 1.2em;
  color: #666;
  margin-bottom: 30px;
  font-family: 'Open Sans', sans-serif;
}

.btn-link {
  display: inline-block;
  margin-top: 20px;
  padding: 12px 24px;
  background-color: #42b983;
  color: white;
  text-decoration: none;
  border-radius: 5px;
  transition: background-color 0.3s, transform 0.3s;
}

.btn-link:hover {
  background-color: #369f6b;
  transform: translateY(-2px);
}

.btn-link:active {
  background-color: #2e8b57;
}
</style> 