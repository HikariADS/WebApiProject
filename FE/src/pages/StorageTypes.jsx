import { useEffect, useState, useMemo } from 'react'
import { storageTypeService } from '../api/storageTypeService'
import { useAuth } from '../contexts/AuthContext'
import { useToast } from '../contexts/ToastContext'
import { handleApiError } from '../utils/errorHandler'
import { ERROR_MESSAGES, SUCCESS_MESSAGES, VALIDATION_MESSAGES } from '../utils/constants'
import './TablePage.css'
import './ProductTypes.css'

const StorageTypes = () => {
  const [storageTypes, setStorageTypes] = useState([])
  const [loading, setLoading] = useState(true)
  const [showModal, setShowModal] = useState(false)
  const [formData, setFormData] = useState({ name: '', managerName: '', storageLocation: '' })
  const [formErrors, setFormErrors] = useState({})
  const [searchTerm, setSearchTerm] = useState('')
  const [editingId, setEditingId] = useState(null)
  const [error, setError] = useState('')
  const { user: currentUser } = useAuth()
  const { showToast } = useToast()

  useEffect(() => {
    fetchStorageTypes()
  }, [])

  const fetchStorageTypes = async () => {
    try {
      setLoading(true)
      const data = await storageTypeService.getAll()
      setStorageTypes(Array.isArray(data) ? data : [])
      setError('')
    } catch (err) {
      const errorMessage = handleApiError(err, ERROR_MESSAGES.FETCH_FAILED, 'StorageTypes.fetchStorageTypes')
      setError(errorMessage)
    } finally {
      setLoading(false)
    }
  }

  const handleOpenModal = (storageType = null) => {
    if (storageType) {
      setFormData({
        name: storageType.name || storageType.Name || '',
        managerName: storageType.managerName || storageType.ManagerName || '',
        storageLocation: storageType.storageLocation || storageType.StorageLocation || ''
      })
      setEditingId(storageType.id || storageType.Id)
    } else {
      setFormData({ name: '', managerName: '', storageLocation: '' })
      setEditingId(null)
    }
    setFormErrors({})
    setShowModal(true)
    setError('')
  }

  const handleCloseModal = () => {
    setShowModal(false)
    setFormData({ name: '', managerName: '', storageLocation: '' })
    setFormErrors({})
    setEditingId(null)
    setError('')
  }

  const validateForm = () => {
    const errors = {}
    
    if (!formData.name.trim()) {
      errors.name = VALIDATION_MESSAGES.REQUIRED
    } else if (formData.name.trim().length < 2) {
      errors.name = VALIDATION_MESSAGES.MIN_LENGTH(2)
    } else if (formData.name.trim().length > 100) {
      errors.name = VALIDATION_MESSAGES.MAX_LENGTH(100)
    }
    
    if (!formData.managerName.trim()) {
      errors.managerName = VALIDATION_MESSAGES.REQUIRED
    } else if (formData.managerName.trim().length < 2) {
      errors.managerName = VALIDATION_MESSAGES.MIN_LENGTH(2)
    } else if (formData.managerName.trim().length > 100) {
      errors.managerName = VALIDATION_MESSAGES.MAX_LENGTH(100)
    }
    
    if (!formData.storageLocation.trim()) {
      errors.storageLocation = VALIDATION_MESSAGES.REQUIRED
    } else if (formData.storageLocation.trim().length < 5) {
      errors.storageLocation = VALIDATION_MESSAGES.MIN_LENGTH(5)
    } else if (formData.storageLocation.trim().length > 225) {
      errors.storageLocation = VALIDATION_MESSAGES.MAX_LENGTH(225)
    }
    
    return errors
  }

  const handleSubmit = async (e) => {
    e.preventDefault()
    setError('')
    setFormErrors({})

    // Validate form
    const validationErrors = validateForm()
    if (Object.keys(validationErrors).length > 0) {
      setFormErrors(validationErrors)
      showToast('Vui lòng kiểm tra lại thông tin đã nhập', 'error')
      return
    }

    try {
      if (editingId) {
        await storageTypeService.update(editingId, formData)
        showToast(SUCCESS_MESSAGES.UPDATED, 'success')
      } else {
        await storageTypeService.create(formData)
        showToast(SUCCESS_MESSAGES.CREATED, 'success')
      }
      handleCloseModal()
      fetchStorageTypes()
    } catch (err) {
      const errorMessage = handleApiError(err, ERROR_MESSAGES.SAVE_FAILED, 'StorageTypes.handleSubmit')
      setError(errorMessage)
      showToast(errorMessage, 'error')
    }
  }

  const handleDelete = async (id) => {
    if (!window.confirm('Bạn có chắc chắn muốn xóa loại kho này?')) {
      return
    }

    try {
      await storageTypeService.delete(id)
      showToast(SUCCESS_MESSAGES.DELETED, 'success')
      fetchStorageTypes()
    } catch (err) {
      const errorMessage = handleApiError(err, ERROR_MESSAGES.DELETE_FAILED, 'StorageTypes.handleDelete')
      showToast(errorMessage, 'error')
    }
  }

  // Filter storage types based on search term
  const filteredStorageTypes = useMemo(() => {
    if (!searchTerm.trim()) return storageTypes
    
    const term = searchTerm.toLowerCase()
    return storageTypes.filter(type => 
      (type.name || type.Name || '').toLowerCase().includes(term) ||
      (type.managerName || type.ManagerName || '').toLowerCase().includes(term) ||
      (type.storageLocation || type.StorageLocation || '').toLowerCase().includes(term)
    )
  }, [storageTypes, searchTerm])

  if (loading) {
    return <div className="loading">Đang tải...</div>
  }

  // Kiểm tra quyền: chỉ Admin và Manager mới có thể thêm/sửa/xóa
  const canEdit = currentUser?.roles?.includes('Admin') || currentUser?.roles?.includes('Manager')

  return (
    <div className="table-page">
      <div className="page-header">
        <h2>Danh sách loại kho</h2>
        <div style={{ display: 'flex', gap: '10px', alignItems: 'center', flexWrap: 'wrap' }}>
          <div className="search-bar" style={{ flex: '1', minWidth: '200px', maxWidth: '400px' }}>
            <input
              type="text"
              placeholder="Tìm kiếm theo tên, người quản lý, vị trí..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              style={{
                width: '100%',
                padding: '8px 12px',
                border: '1px solid #ddd',
                borderRadius: '4px',
                fontSize: '0.9rem'
              }}
            />
          </div>
          {canEdit && (
            <button className="btn-primary" onClick={() => handleOpenModal()}>
              Thêm loại kho
            </button>
          )}
        </div>
      </div>
      {error && !showModal && <div className="error">{error}</div>}
      <div className="table-container">
        <table>
          <thead>
            <tr>
              <th>ID</th>
              <th>Tên loại</th>
              <th>Người quản lý</th>
              <th>Vị trí kho</th>
              <th>Thao tác</th>
            </tr>
          </thead>
          <tbody>
            {filteredStorageTypes.length === 0 ? (
              <tr>
                <td colSpan="5" className="empty-state">
                  {searchTerm ? 'Không tìm thấy kết quả' : 'Chưa có dữ liệu'}
                </td>
              </tr>
            ) : (
              filteredStorageTypes.map((type) => (
                <tr key={type.id || type.Id}>
                  <td>{type.id || type.Id}</td>
                  <td>{type.name || type.Name}</td>
                  <td>{type.managerName || type.ManagerName || '-'}</td>
                  <td>{type.storageLocation || type.StorageLocation || '-'}</td>
                  <td>
                    {canEdit ? (
                      <>
                        <button className="btn-edit" onClick={() => handleOpenModal(type)}>
                          Sửa
                        </button>
                        <button className="btn-delete" onClick={() => handleDelete(type.id || type.Id)}>
                          Xóa
                        </button>
                      </>
                    ) : (
                      <span style={{ color: '#999', fontSize: '0.9rem' }}>Chỉ xem</span>
                    )}
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>

      {showModal && (
        <div className="modal-overlay" onClick={handleCloseModal}>
          <div className="modal-content" onClick={(e) => e.stopPropagation()}>
            <div className="modal-header">
              <h3>{editingId ? 'Sửa loại kho' : 'Thêm loại kho'}</h3>
              <button className="modal-close" onClick={handleCloseModal}>×</button>
            </div>
            <form onSubmit={handleSubmit} noValidate>
              {error && <div className="error-message">{error}</div>}
              <div className="form-group">
                <label htmlFor="name">Tên loại *</label>
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
                  maxLength={100}
                  placeholder="Nhập tên loại kho (ví dụ: Kho lạnh, Kho tổng)"
                  className={formErrors.name ? 'error' : ''}
                />
                {formErrors.name && <span className="field-error">{formErrors.name}</span>}
              </div>
              <div className="form-group">
                <label htmlFor="managerName">Người quản lý *</label>
                <input
                  type="text"
                  id="managerName"
                  value={formData.managerName}
                  onChange={(e) => {
                    setFormData({ ...formData, managerName: e.target.value })
                    if (formErrors.managerName) {
                      setFormErrors({ ...formErrors, managerName: '' })
                    }
                  }}
                  maxLength={100}
                  placeholder="Nhập tên người quản lý"
                  className={formErrors.managerName ? 'error' : ''}
                />
                {formErrors.managerName && <span className="field-error">{formErrors.managerName}</span>}
              </div>
              <div className="form-group">
                <label htmlFor="storageLocation">Vị trí kho *</label>
                <textarea
                  id="storageLocation"
                  value={formData.storageLocation}
                  onChange={(e) => {
                    setFormData({ ...formData, storageLocation: e.target.value })
                    if (formErrors.storageLocation) {
                      setFormErrors({ ...formErrors, storageLocation: '' })
                    }
                  }}
                  maxLength={225}
                  rows={3}
                  placeholder="Nhập vị trí kho (ví dụ: KCN Bắc Thăng Long, Hà Nội)"
                  className={formErrors.storageLocation ? 'error' : ''}
                />
                {formErrors.storageLocation && <span className="field-error">{formErrors.storageLocation}</span>}
                <small style={{ color: '#666', fontSize: '0.85rem', marginTop: '4px', display: 'block' }}>
                  {formData.storageLocation.length}/225 ký tự
                </small>
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

export default StorageTypes

