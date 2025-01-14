<template>
  <div class="app">
    <header>
      <h1>Système de Réservation de Salles</h1>
      <div class="current-time">Heure actuelle : {{ currentTime }}</div>
    </header>
    <main>
      <div class="layout">
        <!-- Formulaire à gauche -->
        <div class="form-container">
          <h2>{{ isEditing ? 'Modifier la réservation' : 'Créer une réservation' }}</h2>
          <form @submit.prevent="isEditing ? updateReservation() : addToUnscheduled()">
            <div class="form-group">
              <label>Titre</label>
              <input 
                type="text" 
                v-model="newReservation.title" 
                placeholder="Nom de la réservation"
                required
              >
            </div>

            <div class="form-group">
              <label>Heure de début</label>
              <input 
                type="time" 
                v-model="newReservation.start"
                min="08:00"
                max="18:00"
                required
              >
            </div>

            <div class="form-group">
              <label>Heure de fin</label>
              <input 
                type="time" 
                v-model="newReservation.end"
                min="08:00"
                max="18:00"
                required
              >
            </div>

            <div class="form-group">
              <label>Description</label>
              <textarea 
                v-model="newReservation.description"
                placeholder="Description optionnelle"
              ></textarea>
            </div>

            <div class="form-group">
              <label>Salle</label>
              <select 
                v-model="newReservation.roomId"
                required
              >
                <option value="">Sélectionner une salle</option>
                <option 
                  v-for="room in rooms" 
                  :key="room.id"
                  :value="room.id"
                >
                  {{ room.name }} ({{ room.capacity }} pers.)
                </option>
              </select>
            </div>

            <div class="form-actions">
              <button type="submit" class="btn-submit">
                {{ isEditing ? 'Sauvegarder' : 'Ajouter aux événements' }}
              </button>
              <button 
                v-if="isEditing" 
                type="button" 
                class="btn-delete" 
                @click="deleteReservation"
              >
                Supprimer
              </button>
              <button 
                v-if="isEditing" 
                type="button" 
                class="btn-cancel" 
                @click="cancelEdit"
              >
                Annuler
              </button>
            </div>
          </form>

          <div class="events-list">
            <h3>Événements à planifier</h3>
            <div 
              v-for="event in unscheduledEvents" 
              :key="event.id"
              class="unscheduled-event"
              draggable="true"
              @dragstart="onDragStart($event, event)"
            >
              <div class="event-header">
                <span class="event-title">{{ event.title }}</span>
                <button @click="removeUnscheduledEvent(event.id)" class="btn-remove">×</button>
              </div>
              <div class="event-details">
                <div>{{ event.start }} - {{ event.end }}</div>
                <div>{{ getRoomName(event.roomId) }}</div>
              </div>
            </div>
          </div>
        </div>

        <!-- Calendrier à droite -->
        <div class="calendar-section">
          <div class="calendar-navigation">
            <button @click="prevWeek" class="nav-btn">←</button>
            <div class="week-selector" @click="showDatePicker = true">
              <span>Semaine du {{ formatDate(weekStart) }} au {{ formatDate(weekEnd) }}</span>
              <VueDatePicker
                v-if="showDatePicker"
                v-model="selectedDate"
                @update:model-value="onDateSelect"
                :enable-time-picker="false"
                auto-apply
                week-start="1"
                locale="fr"
                position="bottom"
                :teleport="true"
                :auto-position="false"
              />
            </div>
            <button @click="nextWeek" class="nav-btn">→</button>
          </div>

          <div class="week-view">
            <div class="weekdays-header">
              <div v-for="(day, index) in weekDays" :key="index" class="weekday">
                {{ day }}
              </div>
            </div>
            <div class="days-grid">
              <div 
                v-for="date in dates" 
                :key="date.toISOString()"
                class="day-cell"
                :class="{ 'current-day': isToday(date) }"
                @dragover.prevent
                @drop="onDrop($event, date)"
              >
                <div class="date-number">{{ date.getDate() }}</div>
                <div class="events-container">
                  <div 
                    v-for="event in getEventsForDate(date)" 
                    :key="event.id"
                    class="calendar-event"
                    draggable="true"
                    @dragstart="onDragStart($event, event)"
                    @click="editEvent(event)"
                  >
                    <div class="event-time">{{ event.start }} - {{ event.end }}</div>
                    <div class="event-title">{{ event.title }}</div>
                    <div class="event-room">{{ getRoomName(event.roomId) }}</div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </main>
    <button @click="exportCalendar" class="btn-export">Exporter le calendrier</button>
  </div>
</template>

<script>
import { ref, computed } from 'vue'
import { useRoomStore } from './store'
import { storeToRefs } from 'pinia'
import VueDatePicker from '@vuepic/vue-datepicker'
import '@vuepic/vue-datepicker/dist/main.css'

export default {
  components: {
    VueDatePicker
  },
  setup() {
    const store = useRoomStore()
    const { rooms } = storeToRefs(store)
    const unscheduledEvents = ref([])
    const draggedEvent = ref(null)

    const isEditing = ref(false)
    const editingEventId = ref(null)
    const newReservation = ref({
      title: '',
      start: '09:00',
      end: '10:00',
      description: '',
      roomId: ''
    })

    const weekDays = ['Lun', 'Mar', 'Mer', 'Jeu', 'Ven', 'Sam', 'Dim']
    const weekStart = ref(getStartOfWeek(new Date()))
    const weekEnd = ref(getEndOfWeek(weekStart.value))

    const dates = computed(() => {
      const dates = []
      const current = new Date(weekStart.value)
      
      for (let i = 0; i < 7; i++) {
        dates.push(new Date(current))
        current.setDate(current.getDate() + 1)
      }
      
      return dates
    })

    function getStartOfWeek(date) {
      const d = new Date(date)
      const day = d.getDay()
      const diff = d.getDate() - day + (day === 0 ? -6 : 1)
      d.setDate(diff)
      return new Date(d)
    }

    function getEndOfWeek(startDate) {
      const end = new Date(startDate)
      end.setDate(end.getDate() + 6)
      return end
    }

    function formatDate(date) {
      if (!date) return ''
      try {
        const d = new Date(date)
        if (isNaN(d.getTime())) return ''
        
        return new Intl.DateTimeFormat('fr-FR', {
          day: 'numeric',
          month: 'long'
        }).format(d)
      } catch (error) {
        console.error('Erreur de formatage de date:', error)
        return ''
      }
    }

    function prevWeek() {
      const newStart = new Date(weekStart.value)
      newStart.setDate(newStart.getDate() - 7)
      weekStart.value = newStart
      weekEnd.value = getEndOfWeek(newStart)
    }

    function nextWeek() {
      const newStart = new Date(weekStart.value)
      newStart.setDate(newStart.getDate() + 7)
      weekStart.value = newStart
      weekEnd.value = getEndOfWeek(newStart)
    }

    function isToday(date) {
      const today = new Date()
      return date.toDateString() === today.toDateString()
    }

    function selectDate(date) {
      console.log('Date sélectionnée:', date)
    }

    function submitReservation() {
      if (!newReservation.value.roomId) {
        alert('Veuillez sélectionner une salle')
        return
      }

      if (!store.isRoomAvailable(
        newReservation.value.roomId,
        weekStart.value,
        newReservation.value.start,
        newReservation.value.end
      )) {
        alert('Cette salle n\'est pas disponible pour ce créneau')
        return
      }

      try {
        store.addReservation({
          ...newReservation.value,
          id: Date.now(),
          date: weekStart.value
        })
        resetForm()
      } catch (error) {
        console.error('Erreur lors de la réservation:', error)
        alert('Erreur lors de la réservation')
      }
    }

    function resetForm() {
      newReservation.value = {
        title: '',
        start: '09:00',
        end: '10:00',
        description: '',
        roomId: ''
      }
    }

    function addToUnscheduled() {
      if (!newReservation.value.title || !newReservation.value.roomId) {
        alert('Veuillez remplir au moins le titre et sélectionner une salle')
        return
      }

      unscheduledEvents.value.push({
        ...newReservation.value,
        id: Date.now()
      })

      resetForm()
    }

    function removeUnscheduledEvent(id) {
      const index = unscheduledEvents.value.findIndex(e => e.id === id)
      if (index !== -1) {
        unscheduledEvents.value.splice(index, 1)
      }
    }

    function onDragStart(event, reservation) {
      draggedEvent.value = reservation
      event.dataTransfer.effectAllowed = 'move'
    }

    function getRoomName(roomId) {
      const room = rooms.value.find(r => r.id === roomId)
      return room ? room.name : ''
    }

    function onDrop(event, date) {
      if (!draggedEvent.value) return

      const newReservation = {
        ...draggedEvent.value,
        date: new Date(date),
        id: draggedEvent.value.id || Date.now()
      }

      try {
        const isAvailable = store.isRoomAvailable(
          newReservation.roomId,
          newReservation.date,
          newReservation.start,
          newReservation.end,
          draggedEvent.value.id
        )

        if (!isAvailable) {
          alert('Cette salle n\'est pas disponible pour ce créneau')
          return
        }

        const isUnscheduled = unscheduledEvents.value.find(e => e.id === draggedEvent.value.id)
        
        if (isUnscheduled) {
          const index = unscheduledEvents.value.findIndex(e => e.id === draggedEvent.value.id)
          if (index !== -1) {
            unscheduledEvents.value.splice(index, 1)
          }
          store.addReservation(newReservation)
        } else {
          store.deleteReservation(draggedEvent.value.id)
          store.addReservation(newReservation)
        }

        draggedEvent.value = null
      } catch (error) {
        console.error('Erreur lors du déplacement:', error)
        alert('Erreur lors de la planification de l\'événement')
      }
    }

    function getEventsForDate(date) {
      if (!date) return []
      return store.reservations.filter(event => {
        const eventDate = new Date(event.date)
        return eventDate.getFullYear() === date.getFullYear() &&
               eventDate.getMonth() === date.getMonth() &&
               eventDate.getDate() === date.getDate()
      })
    }

    const showDatePicker = ref(false)
    const selectedDate = ref(new Date())

    function onDateSelect(date) {
      if (!date) return
      
      try {
        const selectedDate = new Date(date)
        
        const day = selectedDate.getDay()
        const diff = selectedDate.getDate() - day + (day === 0 ? -6 : 1)
        const monday = new Date(selectedDate.setDate(diff))
        
        weekStart.value = new Date(monday)
        weekEnd.value = new Date(monday.setDate(monday.getDate() + 6))
        
        showDatePicker.value = false
      } catch (error) {
        console.error('Erreur lors de la sélection de date:', error)
      }
    }

    function editEvent(event) {
      isEditing.value = true
      editingEventId.value = event.id
      newReservation.value = { ...event }
    }

    function updateReservation() {
      try {
        store.updateReservation({
          ...newReservation.value,
          id: editingEventId.value
        })
        cancelEdit()
      } catch (error) {
        console.error('Erreur lors de la mise à jour:', error)
        alert('Erreur lors de la mise à jour de la réservation')
      }
    }

    function deleteReservation() {
      if (confirm('Êtes-vous sûr de vouloir supprimer cette réservation ?')) {
        store.deleteReservation(editingEventId.value)
        cancelEdit()
      }
    }

    function cancelEdit() {
      isEditing.value = false
      editingEventId.value = null
      resetForm()
    }

    const currentTime = ref(new Date().toLocaleTimeString())
    setInterval(() => {
      currentTime.value = new Date().toLocaleTimeString()
    }, 1000)

    function exportCalendar() {
      const events = store.reservations
      let icsContent = 'BEGIN:VCALENDAR\nVERSION:2.0\nPRODID:-//Your Company//Your Product//EN\n'

      events.forEach(event => {
        icsContent += 'BEGIN:VEVENT\n'
        icsContent += `UID:${event.id}@yourdomain.com\n`
        icsContent += `DTSTAMP:${new Date().toISOString().replace(/[-:]/g, '').split('.')[0]}Z\n`
        icsContent += `DTSTART:${new Date(event.date).toISOString().split('T')[0].replace(/-/g, '')}T${event.start.replace(':', '')}00\n`
        icsContent += `DTEND:${new Date(event.date).toISOString().split('T')[0].replace(/-/g, '')}T${event.end.replace(':', '')}00\n`
        icsContent += `SUMMARY:${event.title}\n`
        icsContent += `DESCRIPTION:${event.description}\n`
        icsContent += 'END:VEVENT\n'
      })

      icsContent += 'END:VCALENDAR'

      const blob = new Blob([icsContent], { type: 'text/calendar' })
      const link = document.createElement('a')
      link.href = URL.createObjectURL(blob)
      link.download = 'calendar.ics'
      link.click()
    }

    return {
      newReservation,
      rooms,
      weekDays,
      weekStart,
      weekEnd,
      dates,
      formatDate,
      prevWeek,
      nextWeek,
      isToday,
      selectDate,
      submitReservation,
      resetForm,
      unscheduledEvents,
      addToUnscheduled,
      removeUnscheduledEvent,
      onDragStart,
      getRoomName,
      onDrop,
      getEventsForDate,
      showDatePicker,
      selectedDate,
      onDateSelect,
      isEditing,
      editEvent,
      updateReservation,
      deleteReservation,
      cancelEdit,
      currentTime,
      exportCalendar
    }
  }
}
</script>

<style>
.app {
  max-width: 1400px;
  margin: 0 auto;
  padding: 20px;
}

.layout {
  display: grid;
  grid-template-columns: 400px 1fr;
  gap: 20px;
  padding: 20px;
  max-width: 1400px;
  margin: 0 auto;
}

.form-container {
  background: white;
  padding: 20px;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}

.calendar-section {
  background: white;
  padding: 20px;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}

.calendar-navigation {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 20px;
  margin-bottom: 20px;
}

.nav-btn {
  background-color: #42b983;
  color: white;
  border: none;
  padding: 8px 16px;
  border-radius: 4px;
  cursor: pointer;
  font-size: 18px;
}

.current-period {
  font-weight: bold;
  font-size: 1.2em;
  min-width: 200px;
  text-align: center;
}

.weekdays-header {
  display: grid;
  grid-template-columns: repeat(7, 1fr);
  background: #f8f9fa;
  border-bottom: 1px solid #dee2e6;
}

.weekday {
  padding: 10px;
  text-align: center;
  font-weight: bold;
}

.days-grid {
  display: grid;
  grid-template-columns: repeat(7, 1fr);
  border-left: 1px solid #dee2e6;
}

.day-cell {
  min-height: 120px;
  padding: 10px;
  border-right: 1px solid #dee2e6;
  border-bottom: 1px solid #dee2e6;
  cursor: pointer;
}

.date-number {
  font-weight: bold;
  margin-bottom: 8px;
}

.current-day {
  background-color: #f8f9fa;
}

.events-container {
  min-height: 80px;
}

/* Styles pour le formulaire existant */
.form-group {
  margin-bottom: 15px;
}

.form-group label {
  display: block;
  margin-bottom: 5px;
  font-weight: bold;
}

.form-group input,
.form-group select,
.form-group textarea {
  width: 100%;
  padding: 8px;
  border: 1px solid #ddd;
  border-radius: 4px;
}

.form-group textarea {
  height: 100px;
  resize: vertical;
}

.btn-submit {
  background-color: #42b983;
  color: white;
  border: none;
  padding: 10px 20px;
  border-radius: 4px;
  cursor: pointer;
  width: 100%;
  margin-top: 20px;
}

.btn-submit:hover {
  background-color: #3aa876;
}

.events-list {
  margin-top: 30px;
  border-top: 1px solid #eee;
  padding-top: 20px;
}

.unscheduled-event {
  background-color: #f8f9fa;
  border: 1px solid #dee2e6;
  border-radius: 4px;
  padding: 10px;
  margin-bottom: 10px;
  cursor: move;
}

.unscheduled-event:hover {
  background-color: #e9ecef;
}

.event-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 5px;
}

.event-title {
  font-weight: bold;
}

.event-details {
  font-size: 0.9em;
  color: #666;
}

.btn-add {
  background-color: #42b983;
  color: white;
  border: none;
  padding: 10px 20px;
  border-radius: 4px;
  cursor: pointer;
  width: 100%;
  margin-top: 20px;
}

.btn-remove {
  background: none;
  border: none;
  color: #dc3545;
  font-size: 1.2em;
  cursor: pointer;
  padding: 0 5px;
}

.btn-remove:hover {
  color: #bd2130;
}

.calendar-event {
  background-color: #42b983;
  color: white;
  padding: 5px 8px;
  border-radius: 4px;
  margin-bottom: 5px;
  font-size: 0.9em;
}

.event-time {
  font-size: 0.8em;
  opacity: 0.9;
}

.event-room {
  font-size: 0.8em;
  opacity: 0.9;
}

.calendar-navigation {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 1rem;
  margin-bottom: 1rem;
}

.week-selector {
  cursor: pointer;
  padding: 0.5rem 1rem;
  border: 1px solid #ddd;
  border-radius: 4px;
  background-color: #fff;
  position: relative;
}

.week-selector:hover {
  background-color: #f8f9fa;
}

.nav-btn {
  padding: 0.5rem 1rem;
  border: 1px solid #ddd;
  border-radius: 4px;
  background-color: #fff;
  cursor: pointer;
}

.nav-btn:hover {
  background-color: #f8f9fa;
}

:deep(.dp__main) {
  position: absolute;
  top: 100%;
  left: 50%;
  transform: translateX(-50%);
  z-index: 1000;
}

.form-actions {
  display: flex;
  gap: 10px;
  margin-top: 20px;
}

.btn-submit {
  background-color: #42b983;
  color: white;
  border: none;
  padding: 10px 20px;
  border-radius: 4px;
  cursor: pointer;
  flex: 1;
}

.btn-delete {
  background-color: #dc3545;
  color: white;
  border: none;
  padding: 10px 20px;
  border-radius: 4px;
  cursor: pointer;
}

.btn-cancel {
  background-color: #6c757d;
  color: white;
  border: none;
  padding: 10px 20px;
  border-radius: 4px;
  cursor: pointer;
}

.btn-submit:hover { background-color: #3aa876; }
.btn-delete:hover { background-color: #c82333; }
.btn-cancel:hover { background-color: #5a6268; }

.calendar-event {
  cursor: pointer;
}
</style>
