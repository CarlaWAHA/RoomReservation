import { defineStore } from 'pinia'

export const useRoomStore = defineStore('room', {
  state: () => ({
    reservations: [],
    rooms: [
      { id: 1, name: 'Salle A', capacity: 10, equipment: 'Projecteur, Tableau blanc' },
      { id: 2, name: 'Salle B', capacity: 20, equipment: 'Écran TV, Système de visioconférence' },
      { id: 3, name: 'Salle C', capacity: 30, equipment: 'Projecteur, Système audio' },
      { id: 4, name: 'Salle D', capacity: 15, equipment: 'Tableau blanc, Wifi haute vitesse' }
    ],
    loading: false,
    error: null
  }),

  getters: {
    getAllRooms: (state) => state.rooms,
    
    getAvailableRooms: (state) => (date, start, end) => {
      return state.rooms.filter(room => {
        return !state.reservations.some(reservation => 
          reservation.roomId === room.id &&
          reservation.date === date &&
          ((start >= reservation.start && start < reservation.end) ||
           (end > reservation.start && end <= reservation.end) ||
           (start <= reservation.start && end >= reservation.end))
        )
      })
    },

    isRoomAvailable: (state) => (roomId, date, start, end, excludeId = null) => {
      const checkDate = new Date(date)
      
      return !state.reservations.some(reservation => {
        // Ignorer la réservation elle-même lors d'un déplacement
        if (excludeId && reservation.id === excludeId) return false
        
        // Vérifier si même salle
        if (reservation.roomId !== roomId) return false
        
        // Vérifier si même date
        const reservationDate = new Date(reservation.date)
        if (reservationDate.getFullYear() !== checkDate.getFullYear() ||
            reservationDate.getMonth() !== checkDate.getMonth() ||
            reservationDate.getDate() !== checkDate.getDate()) {
          return false
        }
        
        // Vérifier le chevauchement horaire
        return (
          (start >= reservation.start && start < reservation.end) ||
          (end > reservation.start && end <= reservation.end) ||
          (start <= reservation.start && end >= reservation.end)
        )
      })
    }
  },

  actions: {
    addReservation(reservation) {
      // Vérification des données requises
      if (!reservation.title || !reservation.start || !reservation.end || !reservation.date || !reservation.roomId) {
        throw new Error('Données de réservation incomplètes')
      }

      // Vérifier la disponibilité de la salle
      if (!this.isRoomAvailable(reservation.roomId, reservation.date, reservation.start, reservation.end)) {
        throw new Error('La salle n\'est pas disponible pour ce créneau')
      }

      // Ajout de la réservation
      this.reservations.push({
        ...reservation,
        id: reservation.id || Date.now()
      })
    },

    updateReservation(updatedReservation) {
      const index = this.reservations.findIndex(r => r.id === updatedReservation.id)
      if (index !== -1) {
        this.reservations[index] = updatedReservation
      }
    },

    deleteReservation(id) {
      const index = this.reservations.findIndex(r => r.id === id)
      if (index !== -1) {
        this.reservations.splice(index, 1)
      }
    },

    // Pour le debug
    getReservations() {
      console.log('Réservations actuelles:', this.reservations)
      return this.reservations
    },

    getReservationsByDate(date) {
      if (!date) return []
      const checkDate = new Date(date).toDateString()
      return this.reservations.filter(reservation => 
        new Date(reservation.date).toDateString() === checkDate
      )
    }
  }
})
