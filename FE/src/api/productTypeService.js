import api from './axiosConfig'

export const productTypeService = {
  getAll: async () => {
    const response = await api.get('/producttype')
    return response.data
  },

  getById: async (id) => {
    const response = await api.get(`/producttype/${id}`)
    return response.data
  },

  create: async (data) => {
    const response = await api.post('/producttype', data)
    return response.data
  },

  update: async (id, data) => {
    const response = await api.put(`/producttype/${id}`, data)
    return response.data
  },

  delete: async (id) => {
    const response = await api.delete(`/producttype/${id}`)
    return response.data
  },
}

