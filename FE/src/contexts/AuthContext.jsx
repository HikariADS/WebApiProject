import { createContext, useState, useEffect, useContext } from 'react'
import { authService } from '../api/authService'

const AuthContext = createContext(null)

export const useAuth = () => {
  const context = useContext(AuthContext)
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider')
  }
  return context
}

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null)
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    const token = localStorage.getItem('token')
    const savedUser = localStorage.getItem('user')
    
    if (token && savedUser) {
      try {
        setUser(JSON.parse(savedUser))
      } catch (error) {
        console.error('Error parsing user data:', error)
        localStorage.removeItem('token')
        localStorage.removeItem('user')
      }
    }
    setLoading(false)
  }, [])

  const login = async (email, password) => {
    try {
      const response = await authService.login(email, password)
      if (response.token || response.Token) {
        const token = response.token || response.Token
        const userData = {
          userId: response.userId || response.UserId,
          userName: response.userName || response.UserName,
          roles: response.roles || response.Roles || [],
        }
        localStorage.setItem('token', token)
        localStorage.setItem('user', JSON.stringify(userData))
        setUser(userData)
        return { success: true }
      }
      return { success: false, error: 'Invalid response' }
    } catch (error) {
      const errorData = error.response?.data
      let errorMessage = 'Đăng nhập thất bại'
      
      if (errorData) {
        if (typeof errorData === 'string') {
          errorMessage = errorData
        } else if (errorData.message) {
          errorMessage = errorData.message
        } else if (errorData.title) {
          errorMessage = errorData.title
        } else if (typeof errorData === 'object') {
          // Nếu là object phức tạp, lấy message hoặc convert thành string
          errorMessage = JSON.stringify(errorData)
        }
      }
      
      return {
        success: false,
        error: errorMessage,
      }
    }
  }

  const register = async (registerData) => {
    try {
      const response = await authService.register(registerData)
      return { success: true, data: response }
    } catch (error) {
      // Backend trả về { errors: [...] } khi đăng ký thất bại
      const errorData = error.response?.data
      let errors = []
      
      if (errorData?.errors) {
        // Nếu errors là mảng
        if (Array.isArray(errorData.errors)) {
          errors = errorData.errors
        } 
        // Nếu errors là object với các property
        else if (typeof errorData.errors === 'object') {
          errors = Object.values(errorData.errors).flat()
        }
      } 
      // Nếu có ModelState errors từ validation
      else if (errorData && typeof errorData === 'object') {
        errors = Object.values(errorData).flat().filter(e => typeof e === 'string')
      }
      // Fallback
      else if (errorData?.message) {
        errors = [errorData.message]
      } else {
        errors = ['Đăng ký thất bại. Vui lòng kiểm tra lại thông tin.']
      }
      
      return {
        success: false,
        errors: errors.length > 0 ? errors : ['Đăng ký thất bại'],
      }
    }
  }

  const logout = () => {
    localStorage.removeItem('token')
    localStorage.removeItem('user')
    setUser(null)
  }

  const value = {
    user,
    login,
    register,
    logout,
    loading,
    isAuthenticated: !!user,
  }

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

