import axios from 'axios';

const api = axios.create({
  baseURL: 'http://localhost:5000/api',
  headers: {
    'Accept': 'application/json',
    'Content-Type': 'application/json'
  },
  withCredentials: false
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
export const createReservation = async (reservation) => {
  try {
    const response = await api.post('/reservations', reservation);
    return response;
  } catch (error) {
    console.error('API Error:', error);
    throw error;
  }
};
export const updateReservation = (id, reservation) => api.put(`/reservations/${id}`, reservation);
export const deleteReservation = (id) => api.delete(`/reservations/${id}`); 
