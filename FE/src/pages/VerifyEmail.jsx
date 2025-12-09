import { useState, useEffect } from 'react'
import { useNavigate, useSearchParams } from 'react-router-dom'
import api from '../api/axiosConfig'
import './Auth.css'

const VerifyEmail = () => {
  const [searchParams] = useSearchParams()
  const navigate = useNavigate()
  const [message, setMessage] = useState('')
  const [loading, setLoading] = useState(true)
  const [success, setSuccess] = useState(false)

  useEffect(() => {
    const verifyEmail = async () => {
      const token = searchParams.get('token')
      
      if (!token) {
        setMessage('Token không hợp lệ')
        setLoading(false)
        return
      }

      try {
        const response = await api.get(`/auth/verify-email?token=${token}`)
        setMessage(response.data.message || 'Email đã được xác thực thành công!')
        setSuccess(true)
      } catch (error) {
        const errorMessage = error.response?.data?.message || 'Xác thực email thất bại'
        setMessage(errorMessage)
        setSuccess(false)
      } finally {
        setLoading(false)
      }
    }

    verifyEmail()
  }, [searchParams])

  if (loading) {
    return (
      <div className="auth-container">
        <div className="auth-card">
          <div style={{ textAlign: 'center', padding: '20px' }}>Đang xác thực email...</div>
        </div>
      </div>
    )
  }

  return (
    <div className="auth-container">
      <div className="auth-card">
        <h2>{success ? 'Xác thực thành công!' : 'Xác thực thất bại'}</h2>
        <div className={success ? 'success-message' : 'error-message'} style={{ marginBottom: '20px' }}>
          {message}
        </div>
        {success && (
          <button 
            className="submit-btn" 
            onClick={() => navigate('/login')}
            style={{ width: '100%' }}
          >
            Đăng nhập ngay
          </button>
        )}
        {!success && (
          <button 
            className="submit-btn" 
            onClick={() => navigate('/register')}
            style={{ width: '100%' }}
          >
            Đăng ký lại
          </button>
        )}
      </div>
    </div>
  )
}

export default VerifyEmail

