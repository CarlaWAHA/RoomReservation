<template>
  <div class="room-list">
    <h2>Salles disponibles</h2>
    <div class="room-grid">
      <div 
        v-for="room in rooms" 
        :key="room.id"
        class="room-card"
        :class="{ 'selected': selectedRoom?.id === room.id }"
        @click="selectRoom(room)"
      >
        <h3>{{ room.name }}</h3>
        <p>Capacité: {{ room.capacity }} personnes</p>
        <p>{{ room.equipment }}</p>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'RoomList',
  data() {
    return {
      selectedRoom: null
    }
  },
  computed: {
    rooms() {
      return this.$store.state.rooms
    }
  },
  methods: {
    selectRoom(room) {
      this.selectedRoom = room
      this.$emit('room-selected', room)
    }
  }
}
</script>

<style scoped>
.room-list {
  padding: 20px;
}

.room-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
  gap: 20px;
  margin-top: 20px;
}

.room-card {
  border: 1px solid #ddd;
  padding: 15px;
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.3s ease;
}

.room-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 6px rgba(0,0,0,0.1);
}

.room-card.selected {
  border-color: #42b983;
  background-color: #f0f9f4;
}
</style>
