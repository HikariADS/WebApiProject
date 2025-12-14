import api from './axiosConfig'

export const authService = {
  login: async (email, password) => {
    const response = await api.post('/auth/login', { 
      EmailorUserName: email, 
      Password: password 
    })
    return response.data
  },

  register: async (registerData) => {
    const response = await api.post('/auth/register', registerData)
    return response.data
  },

  getCurrentUser: async () => {
    const response = await api.get('/auth/me')
    return response.data
  },

  forgotPassword: async (email) => {
    const response = await api.post('/auth/forgot-password', { email })
    return response.data
  },

  resetPassword: async (email, code, newPassword, confirmPassword) => {
    const response = await api.post('/auth/reset-password', {
      email,
      code,
      newPassword,
      confirmPassword
    })
    return response.data
  },
}

