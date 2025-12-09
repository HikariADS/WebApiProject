import api from './axiosConfig'

export const authService = {
  login: async (email, password) => {
    // Backend yêu cầu EmailorUserName và Password (PascalCase)
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
}

