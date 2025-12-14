import api from './axiosConfig'

export const productService = {
  getAll: async (params = {}) => {
    const response = await api.get('/Product', { params })
    return response.data
  },

  getById: async (id) => {
    const response = await api.get(`/Product/${id}`)
    return response.data
  },

  create: async (data) => {
    const response = await api.post('/Product', data)
    return response.data
  },

  update: async (id, data) => {
    const response = await api.put(`/Product/${id}`, data)
    return response.data
  },

  delete: async (id) => {
    const response = await api.delete(`/Product/${id}`)
    return response.data
  },
}

