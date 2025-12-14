import { useEffect, useState } from 'react'
import { userService } from '../api/userService'
import { useAuth } from '../contexts/AuthContext'
import { useToast } from '../contexts/ToastContext'
import { handleApiError, logError } from '../utils/errorHandler'
import { ERROR_MESSAGES, SUCCESS_MESSAGES } from '../utils/constants'
import './TablePage.css'
import './ProductTypes.css'

const Users = () => {
  const [users, setUsers] = useState([])
  const [loading, setLoading] = useState(true)
  const [showModal, setShowModal] = useState(false)
  const [formData, setFormData] = useState({
    userName: '',
    email: '',
    password: '',
    name: '',
    role: 'User',
    unitId: ''
  })
  const [editingId, setEditingId] = useState(null)
  const [error, setError] = useState('')
  const { user: currentUser } = useAuth()
  const [userRole, setUserRole] = useState(null)
  const { showToast } = useToast()

  useEffect(() => {
    // Kiểm tra role của user hiện tại
    if (currentUser?.roles && currentUser.roles.length > 0) {
      setUserRole(currentUser.roles[0]) // Lấy role đầu tiên
    }
    
    // Chỉ fetch nếu user có quyền (Admin hoặc Manager)
    if (currentUser?.roles?.includes('Admin') || currentUser?.roles?.includes('Manager')) {
      fetchUsers()
    } else {
      setLoading(false)
      setError('Bạn không có quyền xem danh sách người dùng. Chỉ Admin và Manager mới có quyền này.')
    }
  }, [currentUser])

  const fetchUsers = async () => {
    try {
      setLoading(true)
      setError('')
      const data = await userService.getAll()
      
      if (Array.isArray(data)) {
        if (data.length > 0) {
          setUsers(data)
        } else {
          setUsers([])
          setError('Chưa có người dùng nào trong hệ thống')
        }
      } else if (data) {
        // Nếu trả về object đơn, convert thành array
        setUsers([data])
      } else {
        setUsers([])
        setError('Không nhận được dữ liệu từ server')
      }
    } catch (err) {
      const errorMessage = handleApiError(err, ERROR_MESSAGES.FETCH_FAILED, 'Users.fetchUsers')
      setError(errorMessage)
      setUsers([])
      
      // Xử lý các loại lỗi đặc biệt
      if (err.response?.status === 403) {
        setError('Bạn không có quyền xem danh sách người dùng')
      } else if (err.response?.status === 401) {
        setError(ERROR_MESSAGES.SESSION_EXPIRED)
      }
    } finally {
      setLoading(false)
    }
  }

  const handleOpenModal = (user = null) => {
    if (user) {
      setFormData({
        userName: user.userName || user.UserName || '',
        email: user.email || user.Email || '',
        password: '',
        name: user.fullName || user.FullName || '',
        role: user.role || user.Role || (user.roles && user.roles.length > 0 ? user.roles[0] : 'User') || 'User',
        unitId: user.unitId || user.UnitId || ''
      })
      setEditingId(user.id || user.Id)
    } else {
      setFormData({
        userName: '',
        email: '',
        password: '',
        name: '',
        role: 'User',
        unitId: ''
      })
      setEditingId(null)
    }
    setShowModal(true)
    setError('')
  }

  const handleCloseModal = () => {
    setShowModal(false)
    setFormData({
      userName: '',
      email: '',
      password: '',
      name: '',
      role: 'User',
      unitId: ''
    })
    setEditingId(null)
    setError('')
  }

  const handleSubmit = async (e) => {
    e.preventDefault()
    setError('')

    // Validation
    if (!editingId && formData.password.length < 6) {
      setError('Mật khẩu phải có ít nhất 6 ký tự')
      return
    }

    try {
      if (editingId) {
        const updateData = {
          name: formData.name,
          email: formData.email,
          unitId: formData.unitId || null
        }
        await userService.update(editingId, updateData)
        showToast(SUCCESS_MESSAGES.UPDATED, 'success')
      } else {
        await userService.create(formData)
        showToast(SUCCESS_MESSAGES.CREATED, 'success')
      }
      handleCloseModal()
      fetchUsers()
    } catch (err) {
      const errorMessage = handleApiError(err, ERROR_MESSAGES.SAVE_FAILED, 'Users.handleSubmit')
      setError(errorMessage)
      showToast(errorMessage, 'error')
    }
  }

  const handleDelete = async (id) => {
    if (!window.confirm('Bạn có chắc chắn muốn xóa người dùng này?')) {
      return
    }

    try {
      await userService.delete(id)
      showToast(SUCCESS_MESSAGES.DELETED, 'success')
      fetchUsers()
    } catch (err) {
      const errorMessage = handleApiError(err, ERROR_MESSAGES.DELETE_FAILED, 'Users.handleDelete')
      showToast(errorMessage, 'error')
    }
  }

  // Kiểm tra quyền truy cập
  const hasAccess = currentUser?.roles?.includes('Admin') || currentUser?.roles?.includes('Manager')
  
  if (!hasAccess) {
    return (
      <div className="table-page">
        <div style={{ padding: '40px', textAlign: 'center' }}>
          <h2>Không có quyền truy cập</h2>
          <p style={{ color: '#666', marginTop: '10px' }}>
            Chỉ Admin và Manager mới có quyền xem danh sách người dùng.
          </p>
        </div>
      </div>
    )
  }

  if (loading) {
    return <div className="loading">Đang tải...</div>
  }

  return (
    <div className="table-page with-card-view">
      <div className="page-header">
        <h2>Danh sách người dùng</h2>
        {(currentUser?.roles?.includes('Admin') || currentUser?.roles?.includes('Manager')) && (
          <button className="btn-primary" onClick={() => handleOpenModal()}>
            Thêm người dùng
          </button>
        )}
      </div>
      {error && !showModal && <div className="error">{error}</div>}
      {users.length === 0 && !loading && !error && (
        <div style={{ padding: '20px', textAlign: 'center', color: '#666' }}>
          Không có dữ liệu. Vui lòng kiểm tra quyền truy cập hoặc thử lại sau.
        </div>
      )}
      <div className="table-container">
        <table>
          <thead>
            <tr>
              <th>ID</th>
              <th>Tên người dùng</th>
              <th>Họ và tên</th>
              <th>Email</th>
              <th>Vai trò</th>
              <th>Thao tác</th>
            </tr>
          </thead>
          <tbody>
            {users.length === 0 ? (
              <tr>
                <td colSpan="6" className="empty-state">
                  Chưa có dữ liệu
                </td>
              </tr>
            ) : (
              users.map((user) => {
                const userId = user.id || user.Id
                const userName = user.userName || user.UserName || ''
                const fullName = user.fullName || user.FullName || ''
                const email = user.email || user.Email || ''
                const role = user.role || user.Role || (user.roles && user.roles.length > 0 ? user.roles[0] : 'User') || 'User'
                
                return (
                  <tr key={userId}>
                    <td>{userId}</td>
                    <td>{userName}</td>
                    <td>{fullName || '-'}</td>
                    <td>{email}</td>
                    <td>
                      {currentUser?.roles?.includes('Admin') ? (
                        // Admin có thể thay đổi role qua dropdown
                        <select
                          value={role}
                          onChange={async (e) => {
                            const newRole = e.target.value
                            const oldRole = role // Lưu role cũ
                            
                            if (window.confirm(`Bạn có chắc chắn muốn thay đổi role của user này từ "${oldRole}" thành "${newRole}"?`)) {
                              try {
                                await userService.changeRole(userId, newRole)
                                showToast('Đã thay đổi role thành công', 'success')
                                // Refresh danh sách sau khi thành công
                                await fetchUsers()
                              } catch (err) {
                                // Reset về giá trị cũ nếu có lỗi
                                e.target.value = oldRole
                                
                                // Hiển thị thông báo lỗi chi tiết
                                const errorMessage = handleApiError(err, 'Không thể thay đổi role', 'Users.handleChangeRole')
                                showToast(errorMessage, 'error')
                              }
                            } else {
                              // Reset về giá trị cũ nếu cancel
                              e.target.value = oldRole
                            }
                          }}
                          style={{
                            padding: '4px 8px',
                            borderRadius: '4px',
                            fontSize: '0.85rem',
                            fontWeight: '500',
                            border: '1px solid #ddd',
                            backgroundColor: role === 'Admin' ? '#fee' : role === 'Manager' ? '#eef' : '#efe',
                            color: role === 'Admin' ? '#c33' : role === 'Manager' ? '#33c' : '#3c3',
                            cursor: 'pointer'
                          }}
                        >
                          <option value="User">User</option>
                          <option value="Manager">Manager</option>
                          <option value="Admin">Admin</option>
                        </select>
                      ) : (
                        // Manager/User chỉ xem role
                        <span style={{
                          padding: '4px 8px',
                          borderRadius: '4px',
                          fontSize: '0.85rem',
                          fontWeight: '500',
                          backgroundColor: role === 'Admin' ? '#fee' : role === 'Manager' ? '#eef' : '#efe',
                          color: role === 'Admin' ? '#c33' : role === 'Manager' ? '#33c' : '#3c3'
                        }}>
                          {role}
                        </span>
                      )}
                    </td>
                    <td>
                      {/* Chỉ Admin và Manager mới có thể sửa/xóa */}
                      {(currentUser?.roles?.includes('Admin') || currentUser?.roles?.includes('Manager')) && (
                        <>
                          <button className="btn-edit" onClick={() => handleOpenModal(user)}>
                            Sửa
                          </button>
                          {/* Chỉ Admin mới có thể xóa user */}
                          {currentUser?.roles?.includes('Admin') && (
                            <button className="btn-delete" onClick={() => handleDelete(userId)}>
                              Xóa
                            </button>
                          )}
                        </>
                      )}
                      {/* User role không có nút thao tác */}
                      {!currentUser?.roles?.includes('Admin') && !currentUser?.roles?.includes('Manager') && (
                        <span style={{ color: '#999', fontSize: '0.9rem' }}>Không có quyền</span>
                      )}
                    </td>
                  </tr>
                )
              })
            )}
          </tbody>
        </table>
      </div>

      {/* Mobile Card View */}
      <div className="card-view">
        {users.length === 0 ? (
          <div className="empty-state">
            {error ? error : 'Chưa có dữ liệu'}
          </div>
        ) : (
          users.map((user) => {
            const userId = user.id || user.Id
            const userName = user.userName || user.UserName || ''
            const fullName = user.fullName || user.FullName || ''
            const email = user.email || user.Email || ''
            const role = user.role || user.Role || (user.roles && user.roles.length > 0 ? user.roles[0] : 'User') || 'User'
            
            return (
              <div key={userId} className="card-item">
                <div className="card-item-header">
                  <div className="card-item-title">{fullName || userName}</div>
                  {(currentUser?.roles?.includes('Admin') || currentUser?.roles?.includes('Manager')) && (
                    <div className="card-item-actions">
                      <button 
                        className="btn-edit" 
                        onClick={() => handleOpenModal(user)}
                        style={{ fontSize: '0.75rem', padding: '4px 8px' }}
                      >
                        Sửa
                      </button>
                      {currentUser?.roles?.includes('Admin') && (
                        <button 
                          className="btn-delete" 
                          onClick={() => handleDelete(userId)}
                          style={{ fontSize: '0.75rem', padding: '4px 8px' }}
                        >
                          Xóa
                        </button>
                      )}
                    </div>
                  )}
                </div>
                <div className="card-item-body">
                  <div className="card-item-row">
                    <span className="card-item-label">ID:</span>
                    <span className="card-item-value">{userId}</span>
                  </div>
                  <div className="card-item-row">
                    <span className="card-item-label">Tên người dùng:</span>
                    <span className="card-item-value">{userName}</span>
                  </div>
                  <div className="card-item-row">
                    <span className="card-item-label">Email:</span>
                    <span className="card-item-value">{email}</span>
                  </div>
                  <div className="card-item-row">
                    <span className="card-item-label">Vai trò:</span>
                    <span className="card-item-value">
                      {currentUser?.roles?.includes('Admin') ? (
                        <select
                          value={role}
                          onChange={async (e) => {
                            const newRole = e.target.value
                            const oldRole = role
                            
                            if (window.confirm(`Bạn có chắc chắn muốn thay đổi role từ "${oldRole}" thành "${newRole}"?`)) {
                              try {
                                await userService.changeRole(userId, newRole)
                                await fetchUsers()
                              } catch (err) {
                                e.target.value = oldRole
                                const errorMessage = err.response?.data?.message || err.message || 'Không thể thay đổi role'
                                alert(`Lỗi: ${errorMessage}`)
                              }
                            } else {
                              e.target.value = oldRole
                            }
                          }}
                          style={{
                            padding: '4px 8px',
                            borderRadius: '4px',
                            fontSize: '0.85rem',
                            border: '1px solid #ddd',
                            backgroundColor: role === 'Admin' ? '#fee' : role === 'Manager' ? '#eef' : '#efe',
                            color: role === 'Admin' ? '#c33' : role === 'Manager' ? '#33c' : '#3c3',
                            cursor: 'pointer'
                          }}
                        >
                          <option value="User">User</option>
                          <option value="Manager">Manager</option>
                          <option value="Admin">Admin</option>
                        </select>
                      ) : (
                        <span style={{
                          padding: '4px 8px',
                          borderRadius: '4px',
                          fontSize: '0.85rem',
                          backgroundColor: role === 'Admin' ? '#fee' : role === 'Manager' ? '#eef' : '#efe',
                          color: role === 'Admin' ? '#c33' : role === 'Manager' ? '#33c' : '#3c3'
                        }}>
                          {role}
                        </span>
                      )}
                    </span>
                  </div>
                </div>
              </div>
            )
          })
        )}
      </div>

      {showModal && (
        <div className="modal-overlay" onClick={handleCloseModal}>
          <div className="modal-content" onClick={(e) => e.stopPropagation()}>
            <div className="modal-header">
              <h3>{editingId ? 'Sửa người dùng' : 'Thêm người dùng'}</h3>
              <button className="modal-close" onClick={handleCloseModal}>×</button>
            </div>
            <form onSubmit={handleSubmit}>
              {error && <div className="error-message">{error}</div>}
              <div className="form-group">
                <label htmlFor="userName">Tên người dùng *</label>
                <input
                  type="text"
                  id="userName"
                  value={formData.userName}
                  onChange={(e) => setFormData({ ...formData, userName: e.target.value })}
                  required
                  maxLength={50}
                  placeholder="Nhập tên người dùng"
                  disabled={!!editingId}
                />
              </div>
              <div className="form-group">
                <label htmlFor="name">Họ và tên *</label>
                <input
                  type="text"
                  id="name"
                  value={formData.name}
                  onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                  required
                  maxLength={50}
                  placeholder="Nhập họ và tên"
                />
              </div>
              <div className="form-group">
                <label htmlFor="email">Email *</label>
                <input
                  type="email"
                  id="email"
                  value={formData.email}
                  onChange={(e) => setFormData({ ...formData, email: e.target.value })}
                  required
                  placeholder="Nhập email"
                />
              </div>
              {!editingId && (
                <div className="form-group">
                  <label htmlFor="password">Mật khẩu *</label>
                  <input
                    type="password"
                    id="password"
                    value={formData.password}
                    onChange={(e) => setFormData({ ...formData, password: e.target.value })}
                    required
                    minLength={6}
                    maxLength={50}
                    placeholder="Nhập mật khẩu (tối thiểu 6 ký tự)"
                  />
                </div>
              )}
              <div className="form-group">
                <label htmlFor="role">Vai trò *</label>
                {editingId ? (
                  // Khi edit, chỉ hiển thị role (không cho chỉnh) - Admin sẽ thay đổi role qua dropdown trong table
                  <input
                    type="text"
                    id="role"
                    value={formData.role}
                    disabled
                    style={{
                      padding: '12px',
                      border: '1px solid #ddd',
                      borderRadius: '5px',
                      backgroundColor: '#f5f5f5',
                      color: '#666',
                      cursor: 'not-allowed'
                    }}
                  />
                ) : (
                  // Khi tạo mới, cho phép chọn role (chỉ Admin mới có thể chọn Admin/Manager)
                  <select
                    id="role"
                    value={formData.role}
                    onChange={(e) => setFormData({ ...formData, role: e.target.value })}
                    required
                  >
                    <option value="User">User</option>
                    {currentUser?.roles?.includes('Admin') && (
                      <>
                        <option value="Manager">Manager</option>
                        <option value="Admin">Admin</option>
                      </>
                    )}
                  </select>
                )}
                {editingId && currentUser?.roles?.includes('Admin') && (
                  <small style={{ color: '#666', display: 'block', marginTop: '4px' }}>
                    Để thay đổi role, vui lòng sử dụng dropdown trong cột "Vai trò" của bảng
                  </small>
                )}
              </div>
              <div className="form-group">
                <label htmlFor="unitId">Unit ID</label>
                <input
                  type="text"
                  id="unitId"
                  value={formData.unitId}
                  onChange={(e) => setFormData({ ...formData, unitId: e.target.value })}
                  placeholder="Nhập Unit ID (tùy chọn)"
                />
              </div>
              <div className="modal-actions">
                <button type="button" className="btn-cancel" onClick={handleCloseModal}>
                  Hủy
                </button>
                <button type="submit" className="btn-primary">
                  {editingId ? 'Cập nhật' : 'Thêm mới'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  )
}

export default Users

