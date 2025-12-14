import { useState } from 'react'
import { useNavigate, Link, useSearchParams } from 'react-router-dom'
import { authService } from '../api/authService'
import { useToast } from '../contexts/ToastContext'
import { logError } from '../utils/errorHandler'
import './Auth.css'

const ResetPassword = () => {
  const [searchParams] = useSearchParams()
  const navigate = useNavigate()
  const { showToast } = useToast()
  
  const [formData, setFormData] = useState({
    email: searchParams.get('email') || '',
    code: '',
    newPassword: '',
    confirmPassword: ''
  })
  const [errors, setErrors] = useState({})
  const [loading, setLoading] = useState(false)

  const handleChange = (e) => {
    const { name, value } = e.target
    setFormData(prev => ({
      ...prev,
      [name]: value
    }))
    // Clear error when user types
    if (errors[name]) {
      setErrors(prev => ({
        ...prev,
        [name]: ''
      }))
    }
  }

  const validateForm = () => {
    const newErrors = {}
    
    if (!formData.email.trim()) {
      newErrors.email = 'Email là bắt buộc'
    } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(formData.email)) {
      newErrors.email = 'Email không hợp lệ'
    }
    
    if (!formData.code.trim()) {
      newErrors.code = 'Mã xác thực là bắt buộc'
    } else if (formData.code.length !== 6) {
      newErrors.code = 'Mã xác thực phải có 6 chữ số'
    }
    
    if (!formData.newPassword.trim()) {
      newErrors.newPassword = 'Mật khẩu mới là bắt buộc'
    } else if (formData.newPassword.length < 6) {
      newErrors.newPassword = 'Mật khẩu phải có ít nhất 6 ký tự'
    }
    
    if (!formData.confirmPassword.trim()) {
      newErrors.confirmPassword = 'Xác nhận mật khẩu là bắt buộc'
    } else if (formData.newPassword !== formData.confirmPassword) {
      newErrors.confirmPassword = 'Mật khẩu xác nhận không khớp'
    }
    
    return newErrors
  }

  const handleSubmit = async (e) => {
    e.preventDefault()
    e.stopPropagation()
    
    const validationErrors = validateForm()
    if (Object.keys(validationErrors).length > 0) {
      setErrors(validationErrors)
      showToast('Vui lòng kiểm tra lại thông tin đã nhập', 'error')
      return
    }

    setErrors({})
    setLoading(true)

    try {
      const response = await authService.resetPassword(
        formData.email,
        formData.code,
        formData.newPassword,
        formData.confirmPassword
      )
      showToast(response.message || 'Đặt lại mật khẩu thành công!', 'success')
      setTimeout(() => {
        navigate('/login')
      }, 2000)
    } catch (err) {
      logError(err, 'ResetPassword.handleSubmit')
      const errorMsg = err.response?.data?.message || 'Có lỗi xảy ra. Vui lòng thử lại.'
      showToast(errorMsg, 'error')
      setErrors({ submit: errorMsg })
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="auth-container">
      <div className="auth-card">
        <h2>Đặt lại mật khẩu</h2>
        {errors.submit && <div className="error-message">{errors.submit}</div>}
        <p style={{ textAlign: 'center', color: '#5BA3D0', marginBottom: '20px' }}>
          Nhập mã xác thực đã được gửi đến email và mật khẩu mới
        </p>
        <form onSubmit={handleSubmit} noValidate>
          <div className="form-group">
            <label htmlFor="email">Email</label>
            <input
              type="email"
              id="email"
              name="email"
              value={formData.email}
              onChange={handleChange}
              required
              placeholder="Nhập email của bạn"
              className={errors.email ? 'error' : ''}
            />
            {errors.email && <span className="field-error">{errors.email}</span>}
          </div>
          <div className="form-group">
            <label htmlFor="code">Mã xác thực</label>
            <input
              type="text"
              id="code"
              name="code"
              value={formData.code}
              onChange={handleChange}
              required
              maxLength={6}
              placeholder="Nhập mã 6 chữ số"
              className={errors.code ? 'error' : ''}
              style={{ letterSpacing: '8px', textAlign: 'center', fontSize: '1.2rem', fontWeight: 'bold' }}
            />
            {errors.code && <span className="field-error">{errors.code}</span>}
          </div>
          <div className="form-group">
            <label htmlFor="newPassword">Mật khẩu mới</label>
            <input
              type="password"
              id="newPassword"
              name="newPassword"
              value={formData.newPassword}
              onChange={handleChange}
              required
              placeholder="Nhập mật khẩu mới (tối thiểu 6 ký tự)"
              className={errors.newPassword ? 'error' : ''}
            />
            {errors.newPassword && <span className="field-error">{errors.newPassword}</span>}
          </div>
          <div className="form-group">
            <label htmlFor="confirmPassword">Xác nhận mật khẩu</label>
            <input
              type="password"
              id="confirmPassword"
              name="confirmPassword"
              value={formData.confirmPassword}
              onChange={handleChange}
              required
              placeholder="Nhập lại mật khẩu mới"
              className={errors.confirmPassword ? 'error' : ''}
            />
            {errors.confirmPassword && <span className="field-error">{errors.confirmPassword}</span>}
          </div>
          <button type="submit" className="submit-btn" disabled={loading}>
            {loading ? 'Đang xử lý...' : 'Đặt lại mật khẩu'}
          </button>
        </form>
        <p className="auth-link">
          <Link to="/login">Quay lại đăng nhập</Link> | <Link to="/forgot-password">Gửi lại mã</Link>
        </p>
      </div>
    </div>
  )
}

export default ResetPassword

