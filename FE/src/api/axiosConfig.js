import axios from 'axios'

const api = axios.create({
  baseURL: '/api',
  headers: {
    'Content-Type': 'application/json',
  },
})

// Request interceptor để thêm token vào header
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token')
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }
    return config
  },
  (error) => {
    return Promise.reject(error)
  }
)

// Response interceptor để xử lý lỗi
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      // Token hết hạn hoặc không hợp lệ
      // Chỉ redirect nếu không phải đang ở trang login/register
      const currentPath = window.location.pathname
      if (currentPath !== '/login' && currentPath !== '/register' && currentPath !== '/verify-email') {
        localStorage.removeItem('token')
        localStorage.removeItem('user')
        // Sử dụng window.location.href để reload và clear state
        window.location.href = '/login'
      }
      // Nếu đang ở trang login/register, chỉ reject error để component xử lý
    }
    return Promise.reject(error)
  }
)

export default api

