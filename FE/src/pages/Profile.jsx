import { useState, useEffect } from 'react'
import { useAuth } from '../contexts/AuthContext'
import { useToast } from '../contexts/ToastContext'
import { userService } from '../api/userService'
import { handleApiError } from '../utils/errorHandler'
import { ERROR_MESSAGES, SUCCESS_MESSAGES, VALIDATION_MESSAGES } from '../utils/constants'
import './Profile.css'

const Profile = () => {
  const { user: currentUser, logout } = useAuth()
  const { showToast } = useToast()
  const [loading, setLoading] = useState(true)
  const [userData, setUserData] = useState(null)
  const [isEditing, setIsEditing] = useState(false)
  const [formData, setFormData] = useState({
    name: '',
    email: '',
    phoneNumber: '',
    userName: ''
  })
  const [formErrors, setFormErrors] = useState({})

  useEffect(() => {
    if (currentUser?.userId) {
      fetchUserData()
    } else {
      setLoading(false)
    }
  }, [currentUser])

  const fetchUserData = async () => {
    try {
      setLoading(true)
      const data = await userService.getById(currentUser.userId)
      setUserData(data)
      setFormData({
        name: data.fullName || data.FullName || '',
        email: data.email || data.Email || '',
        phoneNumber: data.phoneNumber || data.PhoneNumber || '',
        userName: data.userName || data.UserName || ''
      })
    } catch (err) {
      const errorMessage = handleApiError(err, ERROR_MESSAGES.FETCH_FAILED, 'Profile.fetchUserData')
      showToast(errorMessage, 'error')
    } finally {
      setLoading(false)
    }
  }

  const handleEdit = () => {
    setIsEditing(true)
    setFormErrors({})
  }

  const handleCancel = () => {
    setIsEditing(false)
    if (userData) {
      setFormData({
        name: userData.fullName || userData.FullName || '',
        email: userData.email || userData.Email || '',
        phoneNumber: userData.phoneNumber || userData.PhoneNumber || '',
        userName: userData.userName || userData.UserName || ''
      })
    }
    setFormErrors({})
  }

  const validateForm = () => {
    const errors = {}
    
    if (!formData.name.trim()) {
      errors.name = VALIDATION_MESSAGES.REQUIRED
    } else if (formData.name.trim().length < 2) {
      errors.name = VALIDATION_MESSAGES.MIN_LENGTH(2)
    }
    
    if (!formData.email.trim()) {
      errors.email = VALIDATION_MESSAGES.REQUIRED
    } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(formData.email)) {
      errors.email = VALIDATION_MESSAGES.INVALID_EMAIL
    }
    
    if (formData.phoneNumber && !/^[0-9]{10,11}$/.test(formData.phoneNumber.replace(/\s/g, ''))) {
      errors.phoneNumber = VALIDATION_MESSAGES.INVALID_PHONE
    }
    
    return errors
  }

  const handleSubmit = async (e) => {
    e.preventDefault()
    setFormErrors({})

    const validationErrors = validateForm()
    if (Object.keys(validationErrors).length > 0) {
      setFormErrors(validationErrors)
      showToast('Vui lòng kiểm tra lại thông tin đã nhập', 'error')
      return
    }

    try {
      await userService.update(currentUser.userId, {
        name: formData.name,
        email: formData.email,
        phoneNumber: formData.phoneNumber
      })
      showToast(SUCCESS_MESSAGES.UPDATED, 'success')
      setIsEditing(false)
      fetchUserData()
    } catch (err) {
      const errorMessage = handleApiError(err, ERROR_MESSAGES.SAVE_FAILED, 'Profile.handleSubmit')
      showToast(errorMessage, 'error')
    }
  }

  const handleLogout = () => {
    logout()
  }

  if (loading) {
    return <div className="loading">Đang tải...</div>
  }

  if (!userData && !currentUser) {
    return (
      <div className="profile-page">
        <div className="profile-card">
          <h2>Chưa đăng nhập</h2>
          <p>Vui lòng đăng nhập để xem thông tin cá nhân.</p>
        </div>
      </div>
    )
  }

  const userRole = userData?.role || userData?.Role || (userData?.roles && userData.roles[0]) || currentUser?.roles?.[0] || 'User'
  const roleColors = {
    Admin: '#e74c3c',
    Manager: '#3498db',
    User: '#27ae60'
  }

  return (
    <div className="profile-page">
      <div className="profile-header">
        <h1>Thông tin cá nhân</h1>
      </div>

      <div className="profile-card">
        <div className="profile-avatar">
          <div className="avatar-circle">
            {(userData?.fullName || userData?.FullName || userData?.userName || userData?.UserName || 'U').charAt(0).toUpperCase()}
          </div>
          <div className="profile-role" style={{ backgroundColor: roleColors[userRole] || '#95a5a6' }}>
            {userRole}
          </div>
        </div>

        {!isEditing ? (
          <div className="profile-info">
            <div className="info-row">
              <label>Tên đăng nhập</label>
              <div className="info-value">{userData?.userName || userData?.UserName || currentUser?.userName || 'N/A'}</div>
            </div>
            <div className="info-row">
              <label>Họ và tên</label>
              <div className="info-value">{userData?.fullName || userData?.FullName || 'Chưa có'}</div>
            </div>
            <div className="info-row">
              <label>Email</label>
              <div className="info-value">{userData?.email || userData?.Email || currentUser?.email || 'N/A'}</div>
            </div>
            <div className="info-row">
              <label>Số điện thoại</label>
              <div className="info-value">{userData?.phoneNumber || userData?.PhoneNumber || 'Chưa có'}</div>
            </div>
            <div className="info-row">
              <label>Vai trò</label>
              <div className="info-value">
                <span className="role-badge" style={{ backgroundColor: roleColors[userRole] || '#95a5a6' }}>
                  {userRole}
                </span>
              </div>
            </div>
            {userData?.unitId || userData?.UnitId ? (
              <div className="info-row">
                <label>Unit ID</label>
                <div className="info-value">{userData.unitId || userData.UnitId}</div>
              </div>
            ) : null}
            <div className="info-row">
              <label>Ngày tạo tài khoản</label>
              <div className="info-value">
                {userData?.createdDate || userData?.CreatedDate
                  ? new Date(userData.createdDate || userData.CreatedDate).toLocaleDateString('vi-VN')
                  : 'N/A'}
              </div>
            </div>
            <div className="profile-actions">
              <button className="btn-primary" onClick={handleEdit}>
                Chỉnh sửa thông tin
              </button>
              <button className="btn-danger" onClick={handleLogout}>
                Đăng xuất
              </button>
            </div>
          </div>
        ) : (
          <form onSubmit={handleSubmit} className="profile-form">
            <div className="form-group">
              <label htmlFor="userName">Tên đăng nhập</label>
              <input
                type="text"
                id="userName"
                value={formData.userName}
                disabled
                className="disabled-input"
              />
              <small>Tên đăng nhập không thể thay đổi</small>
            </div>
            <div className="form-group">
              <label htmlFor="name">Họ và tên *</label>
              <input
                type="text"
                id="name"
                value={formData.name}
                onChange={(e) => {
                  setFormData({ ...formData, name: e.target.value })
                  if (formErrors.name) {
                    setFormErrors({ ...formErrors, name: '' })
                  }
                }}
                className={formErrors.name ? 'error' : ''}
                placeholder="Nhập họ và tên"
              />
              {formErrors.name && <span className="field-error">{formErrors.name}</span>}
            </div>
            <div className="form-group">
              <label htmlFor="email">Email *</label>
              <input
                type="email"
                id="email"
                value={formData.email}
                onChange={(e) => {
                  setFormData({ ...formData, email: e.target.value })
                  if (formErrors.email) {
                    setFormErrors({ ...formErrors, email: '' })
                  }
                }}
                className={formErrors.email ? 'error' : ''}
                placeholder="Nhập email"
              />
              {formErrors.email && <span className="field-error">{formErrors.email}</span>}
            </div>
            <div className="form-group">
              <label htmlFor="phoneNumber">Số điện thoại</label>
              <input
                type="tel"
                id="phoneNumber"
                value={formData.phoneNumber}
                onChange={(e) => {
                  setFormData({ ...formData, phoneNumber: e.target.value })
                  if (formErrors.phoneNumber) {
                    setFormErrors({ ...formErrors, phoneNumber: '' })
                  }
                }}
                className={formErrors.phoneNumber ? 'error' : ''}
                placeholder="Nhập số điện thoại (tùy chọn)"
              />
              {formErrors.phoneNumber && <span className="field-error">{formErrors.phoneNumber}</span>}
            </div>
            <div className="form-actions">
              <button type="button" className="btn-cancel" onClick={handleCancel}>
                Hủy
              </button>
              <button type="submit" className="btn-primary">
                Lưu thay đổi
              </button>
            </div>
          </form>
        )}
      </div>
    </div>
  )
}

export default Profile

