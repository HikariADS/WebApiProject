import { useEffect, useState } from 'react'
import { storageService } from '../api/storageService'
import { productService } from '../api/productService'
import { storageTypeService } from '../api/storageTypeService'
import { useAuth } from '../contexts/AuthContext'
import './TablePage.css'
import './ProductTypes.css'

const Storage = () => {
  const [storage, setStorage] = useState([])
  const [products, setProducts] = useState([])
  const [storageTypes, setStorageTypes] = useState([])
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

  useEffect(() => {
    fetchStorage()
    fetchProducts()
    fetchStorageTypes()
  }, [])

  const fetchStorage = async () => {
    try {
      setLoading(true)
      const data = await storageService.getAll()
      // Backend trả về array trực tiếp, không phải PageResult
      setStorage(Array.isArray(data) ? data : [])
      setError('')
    } catch (err) {
      setError('Không thể tải danh sách kho')
      console.error(err)
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
      console.error('Error fetching products:', err)
    }
  }

  const fetchStorageTypes = async () => {
    try {
      const data = await storageTypeService.getAll()
      setStorageTypes(Array.isArray(data) ? data : [])
    } catch (err) {
      console.error('Error fetching storage types:', err)
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
      } else {
        await storageService.create(submitData)
      }
      handleCloseModal()
      fetchStorage()
    } catch (err) {
      const errorMessage = err.response?.data?.message || 
                          err.response?.data?.title ||
                          'Có lỗi xảy ra khi lưu kho.'
      setError(errorMessage)
      console.error(err)
    }
  }

  const handleDelete = async (id) => {
    if (!window.confirm('Bạn có chắc chắn muốn xóa kho này?')) {
      return
    }
    try {
      await storageService.delete(id)
      fetchStorage()
    } catch (err) {
      alert('Không thể xóa kho.')
      console.error(err)
    }
  }

  if (loading) {
    return <div className="loading">Đang tải...</div>
  }

  return (
    <div className="table-page">
      <div className="page-header">
        <h2>Danh sách kho</h2>
        <button className="btn-primary" onClick={() => handleOpenModal()}>
          Thêm kho
        </button>
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
                    <button className="btn-edit" onClick={() => handleOpenModal(item)}>
                      Sửa
                    </button>
                    <button className="btn-delete" onClick={() => handleDelete(item.id || item.Id)}>
                      Xóa
                    </button>
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
                <label htmlFor="managerId">Manager ID</label>
                <input
                  type="text"
                  id="managerId"
                  value={formData.managerId}
                  onChange={(e) => setFormData({ ...formData, managerId: e.target.value })}
                  placeholder="Nhập Manager ID (tùy chọn)"
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

export default Storage
