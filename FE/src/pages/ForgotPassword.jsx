import { useState } from 'react'
import { Link } from 'react-router-dom'
import { authService } from '../api/authService'
import { useToast } from '../contexts/ToastContext'
import { logError } from '../utils/errorHandler'
import './Auth.css'

const ForgotPassword = () => {
  const [email, setEmail] = useState('')
  const [error, setError] = useState('')
  const [success, setSuccess] = useState(false)
  const [loading, setLoading] = useState(false)
  const { showToast } = useToast()

  const handleSubmit = async (e) => {
    e.preventDefault()
    e.stopPropagation()
    
    setError('')
    setLoading(true)

    try {
      const response = await authService.forgotPassword(email)
      setSuccess(true)
      showToast(response.message || 'Mã đặt lại mật khẩu đã được gửi đến email của bạn.', 'success')
    } catch (err) {
      logError(err, 'ForgotPassword.handleSubmit')
      const errorMsg = err.response?.data?.message || 'Có lỗi xảy ra. Vui lòng thử lại.'
      setError(errorMsg)
      showToast(errorMsg, 'error')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="auth-container">
      <div className="auth-card">
        <h2>Quên mật khẩu</h2>
        {error && <div className="error-message">{error}</div>}
        {success && (
          <div className="success-message">
            Mã đặt lại mật khẩu đã được gửi đến email của bạn. Vui lòng kiểm tra email và sử dụng mã để đặt lại mật khẩu.
          </div>
        )}
        {!success ? (
          <>
            <p style={{ textAlign: 'center', color: '#5BA3D0', marginBottom: '20px' }}>
              Nhập email của bạn để nhận mã đặt lại mật khẩu
            </p>
            <form onSubmit={handleSubmit} noValidate>
              <div className="form-group">
                <label htmlFor="email">Email</label>
                <input
                  type="email"
                  id="email"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  required
                  placeholder="Nhập email của bạn"
                />
              </div>
              <button type="submit" className="submit-btn" disabled={loading}>
                {loading ? 'Đang gửi...' : 'Gửi mã đặt lại mật khẩu'}
              </button>
            </form>
          </>
        ) : (
          <div style={{ textAlign: 'center' }}>
            <Link to="/reset-password" className="submit-btn" style={{ display: 'inline-block', textDecoration: 'none', marginTop: '20px' }}>
              Đặt lại mật khẩu
            </Link>
          </div>
        )}
        <p className="auth-link">
          <Link to="/login">Quay lại đăng nhập</Link>
        </p>
      </div>
    </div>
  )
}

export default ForgotPassword

