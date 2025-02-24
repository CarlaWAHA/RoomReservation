<template>
  <div class="room-manager">
    <h2>Gestion des Salles</h2>

    <!-- Formulaire d'ajout/modification -->
    <form @submit.prevent="handleSubmit" class="room-form">
      <div class="form-group">
        <label>Nom de la salle</label>
        <input
          v-model="roomForm.name"
          type="text"
          required
          placeholder="Nom de la salle"
        />
      </div>

      <div class="form-group">
        <label>Capacité</label>
        <input v-model="roomForm.capacity" type="number" required min="1" />
      </div>

      <div class="form-group">
        <label>Équipement</label>
        <textarea
          v-model="roomForm.equipment"
          placeholder="Liste des équipements"
        ></textarea>
      </div>

      <button type="submit" class="btn-submit">
        {{ isEditing ? 'Modifier' : 'Ajouter' }} la salle
      </button>

      <button
        v-if="isEditing"
        type="button"
        class="btn-cancel"
        @click="cancelEdit"
      >
        Annuler
      </button>
    </form>

    <!-- Liste des salles -->
    <div class="rooms-list">
      <div v-for="room in store.rooms" :key="room.id" class="room-item">
        <div class="room-info">
          <h3>{{ room.name }}</h3>
          <p>Capacité: {{ room.capacity }} personnes</p>
          <p>Équipement: {{ room.equipment }}</p>
        </div>
        <div class="room-actions">
          <button @click="editRoom(room)" class="btn-edit">Modifier</button>
          <button @click="deleteRoom(room.id)" class="btn-delete">
            Supprimer
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRoomStore } from '../store'

const store = useRoomStore()
const isEditing = ref(false)
const editingId = ref(null)

const roomForm = ref({
  name: '',
  capacity: 1,
  equipment: '',
})

const handleSubmit = async () => {
  try {
    if (isEditing.value) {
      await store.updateRoom(editingId.value, roomForm.value)
    } else {
      await store.addRoom(roomForm.value)
    }
    resetForm()
  } catch (error) {
    console.error('Error submitting room:', error)
    alert('Une erreur est survenue')
  }
}

const editRoom = (room) => {
  isEditing.value = true
  editingId.value = room.id
  roomForm.value = { ...room }
}

const deleteRoom = async (id) => {
  if (confirm('Êtes-vous sûr de vouloir supprimer cette salle ?')) {
    try {
      await store.deleteRoom(id)
    } catch (error) {
      console.error('Error deleting room:', error)
      alert('Une erreur est survenue')
    }
  }
}

const cancelEdit = () => {
  resetForm()
}

const resetForm = () => {
  isEditing.value = false
  editingId.value = null
  roomForm.value = {
    name: '',
    capacity: 1,
    equipment: '',
  }
}
</script>

<style scoped>
.room-manager {
  padding: 20px;
}

.room-form {
  max-width: 500px;
  margin-bottom: 30px;
}

.form-group {
  margin-bottom: 15px;
}

.form-group label {
  display: block;
  margin-bottom: 5px;
}

.form-group input,
.form-group textarea {
  width: 100%;
  padding: 8px;
  border: 1px solid #ddd;
  border-radius: 4px;
}

.rooms-list {
  display: grid;
  gap: 20px;
}

.room-item {
  border: 1px solid #ddd;
  padding: 15px;
  border-radius: 4px;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.room-actions {
  display: flex;
  gap: 10px;
}

.btn-submit,
.btn-edit,
.btn-delete,
.btn-cancel {
  padding: 8px 16px;
  border-radius: 4px;
  cursor: pointer;
  border: none;
}

.btn-submit {
  background-color: #42b983;
  color: white;
}

.btn-edit {
  background-color: #4a9eff;
  color: white;
}

.btn-delete {
  background-color: #ff4a4a;
  color: white;
}

.btn-cancel {
  background-color: #666;
  color: white;
}
</style>
