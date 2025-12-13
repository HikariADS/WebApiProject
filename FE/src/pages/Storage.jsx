import { useEffect, useState } from 'react'
import { storageService } from '../api/storageService'
import { productService } from '../api/productService'
import { storageTypeService } from '../api/storageTypeService'
import { userService } from '../api/userService'
import { useAuth } from '../contexts/AuthContext'
import { useToast } from '../contexts/ToastContext'
import { handleApiError, logError } from '../utils/errorHandler'
import { ERROR_MESSAGES, SUCCESS_MESSAGES } from '../utils/constants'
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
      // Backend trả về array trực tiếp, không phải PageResult
      setStorage(Array.isArray(data) ? data : [])
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
        belongToUnitId: item.belongToUnitId || item.BelongToUnitId || '',
        managerId: item.managerId || item.ManagerId || ''
      })
      setEditingId(item.id || item.Id)
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
    setEditingId(null)
    setError('')
  }

  const handleSubmit = async (e) => {
    e.preventDefault()
    setError('')

    if (!formData.productId || !formData.storageTypeId || !formData.quantity) {
      setError('Vui lòng điền đầy đủ thông tin bắt buộc.')
      return
    }

    if (parseInt(formData.quantity) <= 0) {
      setError('Số lượng phải lớn hơn 0.')
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
        <div style={{ display: 'flex', gap: '10px', alignItems: 'center' }}>
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
            {storage.length === 0 ? (
              <tr>
                <td colSpan="9" className="empty-state">
                  Chưa có kho nào
                </td>
              </tr>
            ) : (
              storage.map((item) => (
                <tr key={item.id || item.Id}>
                  <td>{item.id || item.Id}</td>
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
                        <button className="btn-delete" onClick={() => handleDelete(item.id || item.Id)}>
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
        {storage.length === 0 ? (
          <div className="empty-state">
            {error ? error : 'Chưa có kho nào'}
          </div>
        ) : (
          storage.map((item) => (
            <div key={item.id || item.Id} className="card-item">
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
                      onClick={() => handleDelete(item.id || item.Id)}
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
                  <span className="card-item-value">{item.id || item.Id}</span>
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
            <form onSubmit={handleSubmit}>
              {error && <div className="error-message">{error}</div>}
              <div className="form-group">
                <label htmlFor="productId">Sản phẩm *</label>
                <select
                  id="productId"
                  value={formData.productId}
                  onChange={(e) => setFormData({ ...formData, productId: e.target.value })}
                  required
                >
                  <option value="">Chọn sản phẩm</option>
                  {products.map((product) => (
                    <option key={product.id || product.Id} value={product.id || product.Id}>
                      {product.name || product.Name}
                    </option>
                  ))}
                </select>
              </div>
              <div className="form-group">
                <label htmlFor="storageTypeId">Loại kho *</label>
                <select
                  id="storageTypeId"
                  value={formData.storageTypeId}
                  onChange={(e) => setFormData({ ...formData, storageTypeId: e.target.value })}
                  required
                >
                  <option value="">Chọn loại kho</option>
                  {storageTypes.map((type) => (
                    <option key={type.id || type.Id} value={type.id || type.Id}>
                      {type.name || type.Name}
                    </option>
                  ))}
                </select>
              </div>
              <div className="form-group">
                <label htmlFor="quantity">Số lượng *</label>
                <input
                  type="number"
                  id="quantity"
                  value={formData.quantity}
                  onChange={(e) => setFormData({ ...formData, quantity: e.target.value })}
                  required
                  min="1"
                  placeholder="Nhập số lượng"
                />
              </div>
              <div className="form-group">
                <label htmlFor="importDate">Ngày nhập *</label>
                <input
                  type="date"
                  id="importDate"
                  value={formData.importDate}
                  onChange={(e) => setFormData({ ...formData, importDate: e.target.value })}
                  required
                />
              </div>
              <div className="form-group">
                <label htmlFor="exportDate">Ngày xuất</label>
                <input
                  type="date"
                  id="exportDate"
                  value={formData.exportDate}
                  onChange={(e) => setFormData({ ...formData, exportDate: e.target.value })}
                  placeholder="Ngày xuất (tùy chọn)"
                />
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
                  <option value="">-- Chọn người quản lý --</option>
                  {managers.map((manager) => (
                    <option key={manager.id || manager.Id} value={manager.id || manager.Id}>
                      {manager.fullName || manager.FullName || manager.userName || manager.UserName}
                    </option>
                  ))}
                </select>
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
