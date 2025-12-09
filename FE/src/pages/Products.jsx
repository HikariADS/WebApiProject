import { useEffect, useState } from 'react'
import { productService } from '../api/productService'
import './TablePage.css'

const Products = () => {
  const [products, setProducts] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
    fetchProducts()
  }, [])

  const fetchProducts = async () => {
    try {
      setLoading(true)
      const response = await productService.getAll()
      // Backend returns PageResult with Items property
      setProducts(response.items || response.Items || [])
      setError('')
    } catch (err) {
      setError('Không thể tải danh sách sản phẩm')
      console.error(err)
    } finally {
      setLoading(false)
    }
  }

  if (loading) {
    return <div className="loading">Đang tải...</div>
  }

  if (error) {
    return <div className="error">{error}</div>
  }

  return (
    <div className="table-page">
      <div className="page-header">
        <h2>Danh sách sản phẩm</h2>
        <button className="btn-primary">Thêm sản phẩm</button>
      </div>
      <div className="table-container">
        <table>
          <thead>
            <tr>
              <th>ID</th>
              <th>Tên sản phẩm</th>
              <th>Mô tả</th>
              <th>Giá</th>
              <th>Loại sản phẩm</th>
              <th>Thao tác</th>
            </tr>
          </thead>
          <tbody>
            {products.length === 0 ? (
              <tr>
                <td colSpan="6" className="empty-state">
                  Chưa có sản phẩm nào
                </td>
              </tr>
            ) : (
              products.map((product) => (
                <tr key={product.id || product.Id}>
                  <td>{product.id || product.Id}</td>
                  <td>{product.name || product.Name}</td>
                  <td>{product.description || product.Description || '-'}</td>
                  <td>{product.price || product.Price ? `${(product.price || product.Price).toLocaleString('vi-VN')}đ` : '-'}</td>
                  <td>{product.productTypeName || product.ProductTypeName || '-'}</td>
                  <td>
                    <button className="btn-edit">Sửa</button>
                    <button className="btn-delete">Xóa</button>
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>
    </div>
  )
}

export default Products

