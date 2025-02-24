import { defineStore } from 'pinia'
import { getRooms, getReservations, createReservation, updateReservation, deleteReservation } from '../services/api'

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
      console.log('Checking availability for:', {
        roomId,
        date: checkDate,
        start,
        end,
        excludeId
      })

      return !state.reservations.some(reservation => {
        // Ignorer la réservation elle-même lors d'un déplacement
        if (excludeId && reservation.id === excludeId) return false

        // Vérifier si même salle
        if (reservation.roomId !== roomId) return false

        // Vérifier si même date
        const reservationDate = new Date(reservation.date)
        console.log('Comparing with reservation:', {
          id: reservation.id,
          date: reservationDate,
          start: reservation.start,
          end: reservation.end
        })

        const sameDate =
          reservationDate.getFullYear() === checkDate.getFullYear() &&
          reservationDate.getMonth() === checkDate.getMonth() &&
          reservationDate.getDate() === checkDate.getDate()

        if (!sameDate) return false

        // Vérifier le chevauchement horaire
        const timeOverlap = (
          (start >= reservation.start && start < reservation.end) ||
          (end > reservation.start && end <= reservation.end) ||
          (start <= reservation.start && end >= reservation.end)
        )

        if (timeOverlap) {
          console.log('Time overlap detected')
        }

        return timeOverlap
      })
    }
  },

  actions: {
    async fetchRooms() {
      try {
        this.loading = true
        const response = await getRooms()
        this.rooms = response.data
        console.log('Rooms fetched:', this.rooms)
      } catch (error) {
        console.error('Error fetching rooms:', error)
        this.error = error.message
      } finally {
        this.loading = false
      }
    },

    async fetchReservations() {
      try {
        this.loading = true
        const response = await getReservations()
        this.reservations = response.data
        console.log('Reservations fetched:', this.reservations)
      } catch (error) {
        console.error('Error fetching reservations:', error)
        throw error
      } finally {
        this.loading = false
      }
    },

    async addReservation(reservation) {
      try {
        console.log('Données envoyées à l\'API:', {
          title: reservation.title,
          date: reservation.date,
          start: reservation.start,
          end: reservation.end,
          description: reservation.description,
          roomId: reservation.roomId
        })

        const response = await createReservation({
          title: reservation.title,
          date: reservation.date,
          start: reservation.start,
          end: reservation.end,
          description: reservation.description || '',
          roomId: parseInt(reservation.roomId)
        })

        console.log('Réponse de l\'API:', response.data)
        this.reservations.push(response.data)
        return response.data
      } catch (error) {
        console.error('Détails de l\'erreur:', error.response?.data || error.message)
        throw error
      }
    },

    async updateReservation(id, updatedReservation) {
      try {
        this.loading = true
        console.log('Updating reservation:', id, updatedReservation)

        const response = await updateReservation(id, updatedReservation)

        // Mettre à jour la réservation dans le state
        const index = this.reservations.findIndex(r => r.id === id)
        if (index !== -1) {
          this.reservations[index] = response.data
        }

        // Recharger toutes les réservations pour être sûr
        await this.fetchReservations()

        return response.data
      } catch (error) {
        console.error('Error updating reservation:', error)
        throw error
      } finally {
        this.loading = false
      }
    },

    async deleteReservation(id) {
      try {
        await deleteReservation(id)
        this.reservations = this.reservations.filter(r => r.id !== id)
        console.log('Reservation deleted:', id)
      } catch (error) {
        console.error('Error deleting reservation:', error)
        throw error
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
    },

    // Ajouter un polling pour les mises à jour automatiques
    startPolling() {
      setInterval(() => {
        this.fetchReservations()
      }, 5000) // Rafraîchir toutes les 5 secondes
    }
  }
})
