import api from './axiosConfig'

export const userService = {
  getAll: async () => {
    const response = await api.get('/user')
    return response.data
  },

  getById: async (id) => {
    const response = await api.get(`/user/${id}`)
    return response.data
  },

  create: async (data) => {
    const response = await api.post('/user', data)
    return response.data
  },

  update: async (id, data) => {
    const response = await api.put(`/user/${id}`, data)
    return response.data
  },

  delete: async (id) => {
    const response = await api.delete(`/user/${id}`)
    return response.data
  },

  setManager: async (userId) => {
    const response = await api.post('/user/set-manager', userId)
    return response.data
  },

  changeRole: async (userId, newRole) => {
    const response = await api.put(`/user/${userId}/change-role`, {
      userId: userId,
      newRole: newRole
    })
    return response.data
  },
}

