import api from './axiosConfig'

export const storageService = {
  getAll: async (params = {}) => {
    const response = await api.get('/storage', { params })
    return response.data
  },

  getById: async (id) => {
    const response = await api.get(`/storage/${id}`)
    return response.data
  },

  create: async (data) => {
    const response = await api.post('/storage', data)
    return response.data
  },

  update: async (id, data) => {
    const response = await api.put(`/storage/${id}`, data)
    return response.data
  },

  delete: async (id) => {
    const response = await api.delete(`/storage/${id}`)
    return response.data
  },

  updateManagerIds: async () => {
    const response = await api.post('/storage/update-manager-ids')
    return response.data
  },
}

