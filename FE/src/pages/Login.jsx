import { useState } from 'react'
import { useNavigate, Link } from 'react-router-dom'
import { useAuth } from '../contexts/AuthContext'
import { logError } from '../utils/errorHandler'
import './Auth.css'

const Login = () => {
  const [emailOrUsername, setEmailOrUsername] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)
  const { login, loading: authLoading } = useAuth()
  const navigate = useNavigate()

  // Hiển thị loading nếu AuthContext đang load
  if (authLoading) {
    return (
      <div className="auth-container">
        <div className="auth-card">
          <div style={{ textAlign: 'center', padding: '20px' }}>Đang tải...</div>
        </div>
      </div>
    )
  }

  const handleSubmit = async (e) => {
    e.preventDefault()
    e.stopPropagation()
    
    setError('')
    setLoading(true)

    try {
      const result = await login(emailOrUsername, password)
      
      if (result.success) {
        // Hiển thị thông báo thành công trước khi redirect
        setError('')
        navigate('/')
      } else {
        // Đảm bảo error luôn là string
        const errorMsg = result.error || 'Đăng nhập thất bại'
        setError(typeof errorMsg === 'string' ? errorMsg : 'Đăng nhập thất bại')
        setLoading(false)
      }
    } catch (err) {
      logError(err, 'Login.handleSubmit')
      setError('Có lỗi xảy ra khi đăng nhập. Vui lòng thử lại.')
      setLoading(false)
    }
  }

  return (
    <div className="auth-container">
      <div className="auth-card">
        <h2>Đăng nhập</h2>
        {error && <div className="error-message">{typeof error === 'string' ? error : 'Đăng nhập thất bại'}</div>}
        <form onSubmit={handleSubmit} noValidate>
          <div className="form-group">
            <label htmlFor="emailOrUsername">Email hoặc Tên đăng nhập</label>
            <input
              type="text"
              id="emailOrUsername"
              value={emailOrUsername}
              onChange={(e) => setEmailOrUsername(e.target.value)}
              required
              placeholder="Nhập email hoặc tên đăng nhập"
            />
          </div>
          <div className="form-group">
            <label htmlFor="password">Mật khẩu</label>
            <input
              type="password"
              id="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
              placeholder="Nhập mật khẩu"
            />
            <div style={{ textAlign: 'right', marginTop: '6px' }}>
              <Link 
                to="/forgot-password" 
                className="forgot-password-link"
              >
                Quên mật khẩu?
              </Link>
            </div>
          </div>
          <button type="submit" className="submit-btn" disabled={loading}>
            {loading ? 'Đang đăng nhập...' : 'Đăng nhập'}
          </button>
        </form>
        <p className="auth-link">
          Chưa có tài khoản? <Link to="/register">Đăng ký ngay</Link>
        </p>
      </div>
    </div>
  )
}

export default Login

