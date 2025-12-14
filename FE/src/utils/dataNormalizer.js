/**
 * Normalize data from API to ensure consistent format
 * This helps handle different response formats without exposing backend structure
 */

export const normalizeItem = (item) => {
  if (!item || typeof item !== 'object') return item

  return {
    id: item.id ?? item.Id ?? null,
    title: item.title ?? item.Title ?? '',
    content: item.content ?? item.Content ?? '',
    category: item.category ?? item.Category ?? '',
    authorName: item.authorName ?? item.AuthorName ?? item.author ?? '',
    createdAt: item.createdAt ?? item.CreatedAt ?? null,
    updatedAt: item.updatedAt ?? item.UpdatedAt ?? null,
    ...item
  }
}

export const normalizeArray = (data) => {
  if (!Array.isArray(data)) return []
  return data.map(normalizeItem)
}

export const normalizeResponse = (response) => {
  if (Array.isArray(response)) {
    return normalizeArray(response)
  }
  
  if (response?.items) {
    return {
      items: normalizeArray(response.items),
      totalItems: response.totalItems ?? 0,
      page: response.page ?? 1,
      pageSize: response.pageSize ?? 10
    }
  }
  
  if (response?.Items) {
    return {
      items: normalizeArray(response.Items),
      totalItems: response.TotalItems ?? response.totalItems ?? 0,
      page: response.Page ?? response.page ?? 1,
      pageSize: response.PageSize ?? response.pageSize ?? 10
    }
  }
  
  return normalizeItem(response)
}

