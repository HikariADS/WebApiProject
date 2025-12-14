import { useState } from 'react'
import { useNavigate, Link } from 'react-router-dom'
import { useAuth } from '../contexts/AuthContext'
import { useToast } from '../contexts/ToastContext'
import { logError } from '../utils/errorHandler'
import './Auth.css'

const Register = () => {
  const [formData, setFormData] = useState({
    name: '',
    email: '',
    password: '',
    confirmPassword: '',
    userName: '',
    phoneNumber: '',
  })
  const [errors, setErrors] = useState([])
  const [loading, setLoading] = useState(false)
  const { register } = useAuth()
  const { showToast } = useToast()
  const navigate = useNavigate()

  const handleChange = (e) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value,
    })
  }

  const handleSubmit = async (e) => {
    e.preventDefault()
    e.stopPropagation()
    
    setErrors([])

    // Validation phía client
    const validationErrors = []
    
    if (formData.password !== formData.confirmPassword) {
      validationErrors.push('Mật khẩu không khớp')
    }

    if (formData.password.length < 6) {
      validationErrors.push('Mật khẩu phải có ít nhất 6 ký tự')
    }

    if (validationErrors.length > 0) {
      setErrors(validationErrors)
      return
    }

    setLoading(true)

    try {
      const { confirmPassword, ...registerData } = formData
      const result = await register(registerData)

      if (result.success) {
        // Hiển thị thông báo cần verify email
        showToast('Đăng ký thành công! Vui lòng kiểm tra email để xác thực tài khoản. Link xác thực sẽ hết hạn sau 5 phút.', 'success')
        navigate('/login')
      } else {
        setErrors(result.errors || ['Đăng ký thất bại'])
        setLoading(false)
      }
    } catch (err) {
      logError(err, 'Register.handleSubmit')
      setErrors(['Có lỗi xảy ra khi đăng ký. Vui lòng thử lại.'])
      setLoading(false)
    }
  }

  return (
    <div className="auth-container">
      <div className="auth-card">
        <h2>Đăng ký</h2>
        {errors.length > 0 && (
          <div className="error-message">
            <div className="error-title">Đăng ký thất bại:</div>
            <ul className="error-list">
              {errors.map((error, index) => (
                <li key={index}>{typeof error === 'string' ? error : JSON.stringify(error)}</li>
              ))}
            </ul>
          </div>
        )}
            <form onSubmit={handleSubmit} noValidate>
          <div className="form-group">
            <label htmlFor="name">Họ và tên</label>
            <input
              type="text"
              id="name"
              name="name"
              value={formData.name}
              onChange={handleChange}
              required
              maxLength={30}
              placeholder="Nhập họ và tên"
            />
          </div>
          <div className="form-group">
            <label htmlFor="userName">Tên người dùng</label>
            <input
              type="text"
              id="userName"
              name="userName"
              value={formData.userName}
              onChange={handleChange}
              required
              placeholder="Nhập tên người dùng"
            />
          </div>
          <div className="form-group">
            <label htmlFor="email">Email</label>
            <input
              type="email"
              id="email"
              name="email"
              value={formData.email}
              onChange={handleChange}
              required
              placeholder="Nhập email"
            />
          </div>
          <div className="form-group">
            <label htmlFor="phoneNumber">Số điện thoại</label>
            <input
              type="tel"
              id="phoneNumber"
              name="phoneNumber"
              value={formData.phoneNumber}
              onChange={handleChange}
              placeholder="Nhập số điện thoại (tùy chọn)"
            />
          </div>
          <div className="form-group">
            <label htmlFor="password">Mật khẩu</label>
            <input
              type="password"
              id="password"
              name="password"
              value={formData.password}
              onChange={handleChange}
              required
              placeholder="Nhập mật khẩu"
            />
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
              placeholder="Nhập lại mật khẩu"
            />
          </div>
          <button type="submit" className="submit-btn" disabled={loading}>
            {loading ? 'Đang đăng ký...' : 'Đăng ký'}
          </button>
        </form>
        <p className="auth-link">
          Đã có tài khoản? <Link to="/login">Đăng nhập ngay</Link>
        </p>
      </div>
    </div>
  )
}

export default Register

