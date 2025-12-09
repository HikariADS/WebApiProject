import { useState } from 'react'
import { useNavigate, Link } from 'react-router-dom'
import { useAuth } from '../contexts/AuthContext'
import './Auth.css'

const Login = () => {
  const [email, setEmail] = useState('')
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
    setError('')
    setLoading(true)

    const result = await login(email, password)
    
    if (result.success) {
      navigate('/')
    } else {
      // Đảm bảo error luôn là string
      const errorMsg = result.error
      setError(typeof errorMsg === 'string' ? errorMsg : 'Đăng nhập thất bại')
    }
    
    setLoading(false)
  }

  return (
    <div className="auth-container">
      <div className="auth-card">
        <h2>Đăng nhập</h2>
        {error && <div className="error-message">{typeof error === 'string' ? error : 'Đăng nhập thất bại'}</div>}
        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label htmlFor="email">Email</label>
            <input
              type="email"
              id="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
              placeholder="Nhập email"
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

