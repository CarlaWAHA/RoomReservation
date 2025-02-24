import axios from 'axios';

const api = axios.create({
  baseURL: 'http://localhost:5000/api',
  headers: {
    'Accept': 'application/json',
    'Content-Type': 'application/json'
  },
  withCredentials: false // Désactive les credentials
});

// Intercepteur pour gérer les erreurs
api.interceptors.response.use(
  response => response,
  error => {
    console.error('API Error:', error);
    return Promise.reject(error);
  }
);

export const getRooms = () => api.get('/rooms');
export const getReservations = () => api.get('/reservations');
export const createReservation = (reservation) => api.post('/reservations', reservation);
export const updateReservation = (id, reservation) => api.put(`/reservations/${id}`, reservation);
export const deleteReservation = (id) => api.delete(`/reservations/${id}`); 
