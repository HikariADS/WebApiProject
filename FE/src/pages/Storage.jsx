import { useEffect, useState, useMemo } from 'react'
import { storageService } from '../api/storageService'
import { productService } from '../api/productService'
import { storageTypeService } from '../api/storageTypeService'
import { userService } from '../api/userService'
import { useAuth } from '../contexts/AuthContext'
import { useToast } from '../contexts/ToastContext'
import { handleApiError, logError } from '../utils/errorHandler'
import { normalizeArray } from '../utils/dataNormalizer'
import { ERROR_MESSAGES, SUCCESS_MESSAGES, VALIDATION_MESSAGES } from '../utils/constants'
import './TablePage.css'
import './ProductTypes.css'

const Storage = () => {
  const [storage, setStorage] = useState([])
  const [products, setProducts] = useState([])
  const [storageTypes, setStorageTypes] = useState([])
  const [managers, setManagers] = useState([])
  const [loading, setLoading] = useState(true)
  const [showModal, setShowModal] = useState(false)
  const [formData, setFormData] = useState({
    productId: '',
    storageTypeId: '',
    quantity: '',
    importDate: new Date().toISOString().split('T')[0],
    exportDate: '',
    belongToUnitId: '',
    managerId: ''
  })
  const [editingId, setEditingId] = useState(null)
  const [error, setError] = useState('')
  const [searchTerm, setSearchTerm] = useState('')
  const [formErrors, setFormErrors] = useState({})
  const { user: currentUser } = useAuth()
  const { showToast } = useToast()

  useEffect(() => {
    fetchStorage()
    fetchProducts()
    fetchStorageTypes()
    fetchManagers()
  }, [])

  const fetchStorage = async () => {
    try {
      setLoading(true)
      const data = await storageService.getAll()
      setStorage(normalizeArray(data))
      setError('')
    } catch (err) {
      const errorMessage = handleApiError(err, ERROR_MESSAGES.FETCH_FAILED, 'Storage.fetchStorage')
      setError(errorMessage)
      if (err.response?.status === 403) {
        setError('Bạn không có quyền xem danh sách kho. Chỉ Admin và Manager mới có quyền này.')
      }
    } finally {
      setLoading(false)
    }
  }

  const fetchProducts = async () => {
    try {
      const response = await productService.getAll()
      const productsData = response.items || response.Items || []
      setProducts(Array.isArray(productsData) ? productsData : [])
    } catch (err) {
      logError(err, 'Storage.fetchProducts')
      // Silently fail - products are optional for storage form
    }
  }

  const fetchStorageTypes = async () => {
    try {
      const data = await storageTypeService.getAll()
      setStorageTypes(Array.isArray(data) ? data : [])
    } catch (err) {
      logError(err, 'Storage.fetchStorageTypes')
      showToast('Không thể tải danh sách loại kho', 'error')
    }
  }

  const fetchManagers = async () => {
    try {
      const data = await userService.getAll()
      const allUsers = Array.isArray(data) ? data : []
      // Lọc chỉ lấy users có role Manager hoặc Admin
      const managerUsers = allUsers.filter(user => {
        const role = user.role || user.Role || (user.roles && user.roles.length > 0 ? user.roles[0] : '')
        return role === 'Manager' || role === 'Admin'
      })
      setManagers(managerUsers)
    } catch (err) {
      logError(err, 'Storage.fetchManagers')
      // Silently fail - managers are optional for storage form
    }
  }

  const handleOpenModal = (item = null) => {
    if (item) {
      setFormData({
        productId: item.productId || item.ProductId || '',
        storageTypeId: item.storageTypeId || item.StorageTypeId || '',
        quantity: item.quantity || item.Quantity || '',
        importDate: item.importDate ? new Date(item.importDate).toISOString().split('T')[0] : new Date().toISOString().split('T')[0],
        exportDate: item.exportDate ? new Date(item.exportDate).toISOString().split('T')[0] : '',
        belongToUnitId: item.belongToUnitId || '',
        managerId: item.managerId || ''
      })
      setEditingId(item.id)
    } else {
      setFormData({
        productId: '',
        storageTypeId: '',
        quantity: '',
        importDate: new Date().toISOString().split('T')[0],
        exportDate: '',
        belongToUnitId: currentUser?.unitId || '',
        managerId: ''
      })
      setEditingId(null)
    }
    setFormErrors({})
    setShowModal(true)
    setError('')
  }

  const handleCloseModal = () => {
    setShowModal(false)
    setFormData({
      productId: '',
      storageTypeId: '',
      quantity: '',
      importDate: new Date().toISOString().split('T')[0],
      exportDate: '',
      belongToUnitId: '',
      managerId: ''
    })
    setFormErrors({})
    setEditingId(null)
    setError('')
  }

  const validateForm = () => {
    const errors = {}
    
    if (!formData.productId) {
      errors.productId = VALIDATION_MESSAGES.REQUIRED
    }
    
    if (!formData.storageTypeId) {
      errors.storageTypeId = VALIDATION_MESSAGES.REQUIRED
    }
    
    if (!formData.quantity) {
      errors.quantity = VALIDATION_MESSAGES.REQUIRED
    } else {
      const quantity = parseInt(formData.quantity)
      if (isNaN(quantity) || quantity <= 0) {
        errors.quantity = VALIDATION_MESSAGES.MIN_VALUE(1)
      }
    }
    
    if (!formData.importDate) {
      errors.importDate = VALIDATION_MESSAGES.REQUIRED
    } else {
      const importDate = new Date(formData.importDate)
      const today = new Date()
      today.setHours(0, 0, 0, 0)
      if (importDate > today) {
        errors.importDate = 'Ngày nhập không được lớn hơn ngày hiện tại'
      }
    }
    
    if (formData.exportDate) {
      const exportDate = new Date(formData.exportDate)
      const importDate = new Date(formData.importDate)
      if (exportDate < importDate) {
        errors.exportDate = 'Ngày xuất phải lớn hơn hoặc bằng ngày nhập'
      }
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
      const submitData = {
        productId: parseInt(formData.productId),
        storageTypeId: parseInt(formData.storageTypeId),
        quantity: parseInt(formData.quantity),
        importDate: new Date(formData.importDate).toISOString(),
        exportDate: formData.exportDate ? new Date(formData.exportDate).toISOString() : null,
        belongToUnitId: formData.belongToUnitId || null,
        managerId: formData.managerId || null
      }

      if (editingId) {
        submitData.id = editingId
        await storageService.update(editingId, submitData)
        showToast(SUCCESS_MESSAGES.UPDATED, 'success')
      } else {
        await storageService.create(submitData)
        showToast(SUCCESS_MESSAGES.CREATED, 'success')
      }
      handleCloseModal()
      fetchStorage()
    } catch (err) {
      const errorMessage = handleApiError(err, ERROR_MESSAGES.SAVE_FAILED, 'Storage.handleSubmit')
      setError(errorMessage)
      showToast(errorMessage, 'error')
    }
  }

  const handleDelete = async (id) => {
    if (!window.confirm('Bạn có chắc chắn muốn xóa kho này?')) {
      return
    }
    try {
      await storageService.delete(id)
      showToast(SUCCESS_MESSAGES.DELETED, 'success')
      fetchStorage()
    } catch (err) {
      const errorMessage = handleApiError(err, ERROR_MESSAGES.DELETE_FAILED, 'Storage.handleDelete')
      showToast(errorMessage, 'error')
    }
  }

  const handleUpdateManagerIds = async () => {
    if (!window.confirm('Bạn có chắc chắn muốn cập nhật ManagerId cho tất cả các kho chưa có người quản lý?')) {
      return
    }
    try {
      setLoading(true)
      const result = await storageService.updateManagerIds()
      const message = result.message || 'Đã cập nhật ManagerId cho các kho.'
      showToast(message, 'success')
      fetchStorage() // Refresh danh sách
    } catch (err) {
      const errorMessage = handleApiError(err, 'Không thể cập nhật ManagerId', 'Storage.handleUpdateManagerIds')
      showToast(errorMessage, 'error')
    } finally {
      setLoading(false)
    }
  }

  // Filter storage based on search term
  const filteredStorage = useMemo(() => {
    if (!searchTerm.trim()) return storage
    
    const term = searchTerm.toLowerCase()
    return storage.filter(item => 
      (item.productName || item.ProductName || '').toLowerCase().includes(term) ||
      (item.storageTypeName || item.StorageTypeName || '').toLowerCase().includes(term) ||
      (item.storageLocation || item.StorageLocation || '').toLowerCase().includes(term) ||
      (item.managerName || item.ManagerName || '').toLowerCase().includes(term) ||
      String(item.quantity || item.Quantity || '').includes(term)
    )
  }, [storage, searchTerm])

  if (loading) {
    return <div className="loading">Đang tải...</div>
  }

  // Kiểm tra quyền: chỉ Admin và Manager mới có thể thêm/sửa/xóa
  const canEdit = currentUser?.roles?.includes('Admin') || currentUser?.roles?.includes('Manager')
  const isAdmin = currentUser?.roles?.includes('Admin')

  return (
    <div className="table-page with-card-view">
      <div className="page-header">
        <h2>Danh sách kho</h2>
        <div style={{ display: 'flex', gap: '10px', alignItems: 'center', flexWrap: 'wrap' }}>
          <div className="search-bar" style={{ flex: '1', minWidth: '200px', maxWidth: '400px' }}>
            <input
              type="text"
              placeholder="Tìm kiếm theo sản phẩm, loại kho, vị trí, người quản lý..."
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
          {isAdmin && (
            <button 
              className="btn-primary" 
              onClick={handleUpdateManagerIds}
              style={{ backgroundColor: '#27ae60', fontSize: '0.9rem', padding: '8px 16px' }}
              title="Cập nhật ManagerId cho các kho chưa có người quản lý"
            >
              Cập nhật Manager
            </button>
          )}
          {canEdit && (
            <button className="btn-primary" onClick={() => handleOpenModal()}>
              Thêm kho
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
              <th>Sản phẩm</th>
              <th>Loại kho</th>
              <th>Vị trí</th>
              <th>Số lượng</th>
              <th>Ngày nhập</th>
              <th>Ngày xuất</th>
              <th>Người quản lý</th>
              <th>Thao tác</th>
            </tr>
          </thead>
          <tbody>
            {filteredStorage.length === 0 ? (
              <tr>
                <td colSpan="9" className="empty-state">
                  {searchTerm ? 'Không tìm thấy kết quả' : 'Chưa có kho nào'}
                </td>
              </tr>
            ) : (
              filteredStorage.map((item) => (
                <tr key={item.id}>
                  <td>{item.id}</td>
                  <td>{item.productName || item.ProductName || '-'}</td>
                  <td>{item.storageTypeName || item.StorageTypeName || '-'}</td>
                  <td>{item.storageLocation || item.StorageLocation || '-'}</td>
                  <td>{item.quantity || item.Quantity || 0}</td>
                  <td>{item.importDate ? new Date(item.importDate).toLocaleDateString('vi-VN') : '-'}</td>
                  <td>{item.exportDate ? new Date(item.exportDate).toLocaleDateString('vi-VN') : '-'}</td>
                  <td>{item.managerName || item.ManagerName || '-'}</td>
                  <td>
                    {canEdit ? (
                      <>
                        <button className="btn-edit" onClick={() => handleOpenModal(item)}>
                          Sửa
                        </button>
                        <button className="btn-delete" onClick={() => handleDelete(item.id)}>
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

      {/* Mobile Card View */}
      <div className="card-view">
        {filteredStorage.length === 0 ? (
          <div className="empty-state">
            {searchTerm ? 'Không tìm thấy kết quả' : (error ? error : 'Chưa có kho nào')}
          </div>
        ) : (
          filteredStorage.map((item) => (
            <div key={item.id} className="card-item">
              <div className="card-item-header">
                <div className="card-item-title">{item.productName || item.ProductName || '-'}</div>
                {canEdit && (
                  <div className="card-item-actions">
                    <button 
                      className="btn-edit" 
                      onClick={() => handleOpenModal(item)}
                      style={{ fontSize: '0.75rem', padding: '4px 8px' }}
                    >
                      Sửa
                    </button>
                    <button 
                      className="btn-delete" 
                      onClick={() => handleDelete(item.id)}
                      style={{ fontSize: '0.75rem', padding: '4px 8px' }}
                    >
                      Xóa
                    </button>
                  </div>
                )}
              </div>
              <div className="card-item-body">
                <div className="card-item-row">
                  <span className="card-item-label">ID:</span>
                  <span className="card-item-value">{item.id}</span>
                </div>
                <div className="card-item-row">
                  <span className="card-item-label">Loại kho:</span>
                  <span className="card-item-value">{item.storageTypeName || item.StorageTypeName || '-'}</span>
                </div>
                <div className="card-item-row">
                  <span className="card-item-label">Vị trí:</span>
                  <span className="card-item-value">
                    {(item.storageLocation || item.StorageLocation || '-').substring(0, 40)}
                    {(item.storageLocation || item.StorageLocation || '').length > 40 ? '...' : ''}
                  </span>
                </div>
                <div className="card-item-row">
                  <span className="card-item-label">Số lượng:</span>
                  <span className="card-item-value">{item.quantity || item.Quantity || 0}</span>
                </div>
                <div className="card-item-row">
                  <span className="card-item-label">Ngày nhập:</span>
                  <span className="card-item-value">
                    {item.importDate ? new Date(item.importDate).toLocaleDateString('vi-VN') : '-'}
                  </span>
                </div>
                <div className="card-item-row">
                  <span className="card-item-label">Ngày xuất:</span>
                  <span className="card-item-value">
                    {item.exportDate ? new Date(item.exportDate).toLocaleDateString('vi-VN') : '-'}
                  </span>
                </div>
                <div className="card-item-row">
                  <span className="card-item-label">Người quản lý:</span>
                  <span className="card-item-value">
                    {item.managerName || item.ManagerName || '-'}
                  </span>
                </div>
              </div>
            </div>
          ))
        )}
      </div>

      {showModal && (
        <div className="modal-overlay" onClick={handleCloseModal}>
          <div className="modal-content" onClick={(e) => e.stopPropagation()}>
            <div className="modal-header">
              <h3>{editingId ? 'Sửa kho' : 'Thêm kho'}</h3>
              <button className="modal-close" onClick={handleCloseModal}>×</button>
            </div>
            <form onSubmit={handleSubmit} noValidate>
              {error && <div className="error-message">{error}</div>}
              <div className="form-group">
                <label htmlFor="productId">Sản phẩm *</label>
                <select
                  id="productId"
                  value={formData.productId}
                  onChange={(e) => {
                    setFormData({ ...formData, productId: e.target.value })
                    if (formErrors.productId) {
                      setFormErrors({ ...formErrors, productId: '' })
                    }
                  }}
                  className={formErrors.productId ? 'error' : ''}
                >
                  <option value="">Chọn sản phẩm</option>
                  {products.map((product) => (
                    <option key={product.id || product.Id} value={product.id || product.Id}>
                      {product.name || product.Name}
                    </option>
                  ))}
                </select>
                {formErrors.productId && <span className="field-error">{formErrors.productId}</span>}
              </div>
              <div className="form-group">
                <label htmlFor="storageTypeId">Loại kho *</label>
                <select
                  id="storageTypeId"
                  value={formData.storageTypeId}
                  onChange={(e) => {
                    setFormData({ ...formData, storageTypeId: e.target.value })
                    if (formErrors.storageTypeId) {
                      setFormErrors({ ...formErrors, storageTypeId: '' })
                    }
                  }}
                  className={formErrors.storageTypeId ? 'error' : ''}
                >
                  <option value="">Chọn loại kho</option>
                  {storageTypes.map((type) => (
                    <option key={type.id || type.Id} value={type.id || type.Id}>
                      {type.name || type.Name}
                    </option>
                  ))}
                </select>
                {formErrors.storageTypeId && <span className="field-error">{formErrors.storageTypeId}</span>}
              </div>
              <div className="form-group">
                <label htmlFor="quantity">Số lượng *</label>
                <input
                  type="number"
                  id="quantity"
                  value={formData.quantity}
                  onChange={(e) => {
                    setFormData({ ...formData, quantity: e.target.value })
                    if (formErrors.quantity) {
                      setFormErrors({ ...formErrors, quantity: '' })
                    }
                  }}
                  min="1"
                  placeholder="Nhập số lượng (phải lớn hơn 0)"
                  className={formErrors.quantity ? 'error' : ''}
                />
                {formErrors.quantity && <span className="field-error">{formErrors.quantity}</span>}
              </div>
              <div className="form-group">
                <label htmlFor="importDate">Ngày nhập *</label>
                <input
                  type="date"
                  id="importDate"
                  value={formData.importDate}
                  onChange={(e) => {
                    setFormData({ ...formData, importDate: e.target.value })
                    if (formErrors.importDate) {
                      setFormErrors({ ...formErrors, importDate: '' })
                    }
                  }}
                  max={new Date().toISOString().split('T')[0]}
                  className={formErrors.importDate ? 'error' : ''}
                />
                {formErrors.importDate && <span className="field-error">{formErrors.importDate}</span>}
              </div>
              <div className="form-group">
                <label htmlFor="exportDate">Ngày xuất</label>
                <input
                  type="date"
                  id="exportDate"
                  value={formData.exportDate}
                  onChange={(e) => {
                    setFormData({ ...formData, exportDate: e.target.value })
                    if (formErrors.exportDate) {
                      setFormErrors({ ...formErrors, exportDate: '' })
                    }
                  }}
                  min={formData.importDate || undefined}
                  placeholder="Ngày xuất (tùy chọn)"
                  className={formErrors.exportDate ? 'error' : ''}
                />
                {formErrors.exportDate && <span className="field-error">{formErrors.exportDate}</span>}
                <small style={{ color: '#666', fontSize: '0.85rem', marginTop: '4px', display: 'block' }}>
                  Ngày xuất phải lớn hơn hoặc bằng ngày nhập
                </small>
              </div>
              <div className="form-group">
                <label htmlFor="belongToUnitId">Unit ID</label>
                <input
                  type="text"
                  id="belongToUnitId"
                  value={formData.belongToUnitId}
                  onChange={(e) => setFormData({ ...formData, belongToUnitId: e.target.value })}
                  placeholder="Nhập Unit ID (tùy chọn)"
                />
              </div>
              <div className="form-group">
                <label htmlFor="managerId">Người quản lý</label>
                <select
                  id="managerId"
                  value={formData.managerId}
                  onChange={(e) => setFormData({ ...formData, managerId: e.target.value })}
                >
                  <option value="">-- Chọn người quản lý (tùy chọn) --</option>
                  {managers.map((manager) => (
                    <option key={manager.id || manager.Id} value={manager.id || manager.Id}>
                      {manager.fullName || manager.FullName || manager.userName || manager.UserName}
                    </option>
                  ))}
                </select>
                <small style={{ color: '#666', fontSize: '0.85rem', marginTop: '4px', display: 'block' }}>
                  Chọn người quản lý cho kho này (tùy chọn)
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

export default Storage
