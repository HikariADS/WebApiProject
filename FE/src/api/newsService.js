import api from './axiosConfig'

export const newsService = {
  getAll: async () => {
    const response = await api.get('/news')
    return response.data
  },

  getById: async (id) => {
    const response = await api.get(`/news/${id}`)
    return response.data
  },

  create: async (data) => {
    const response = await api.post('/news', data)
    return response.data
  },

  update: async (id, data) => {
    const response = await api.put(`/news/${id}`, data)
    return response.data
  },

  delete: async (id) => {
    const response = await api.delete(`/news/${id}`)
    return response.data
  },
}

