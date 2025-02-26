import axios from 'axios'

const API_URL = 'http://localhost:5000/api'

export const sendEmail = async (emailData) => {
  try {
    console.log('Simulation d\'envoi d\'email:', emailData);
    await new Promise(resolve => setTimeout(resolve, 1000));
    return { success: true, message: 'Email simulé envoyé avec succès' };
  } catch (error) {
    console.error("Erreur lors de l'envoi de l'email:", error);
    throw error;
  }
} 
