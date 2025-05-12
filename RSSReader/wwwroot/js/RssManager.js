document.getElementById('feedItemsModal')?.addEventListener('show.bs.modal', async function (event)
{
    const button = event.relatedTarget;
    const feedId = button.getAttribute('data-feed-id');
    const feedTitle = button.getAttribute('data-feed-title');
    const modalBody = document.getElementById('feedItemsModalBody');

    modalBody.innerHTML = '<div class="text-center text-muted">Загрузка новостей...</div>';

    try {
        const response = await fetch(`/RssManager?handler=FeedItemsJson&feedId=${feedId}`);
        const items = await response.json();

        if (!items?.length) {
            modalBody.innerHTML = '<div class="alert alert-info">Новостей нет.</div>';
            return;
        }

        let html = '<div class="feed-modal-card">';
        items.forEach(item => {
            html += `
                <div class="card mb-3 shadow-sm">
                    <div class="card-body">
                        <h5 class="card-title">${item.title}</h5>
                        ${item.description ? `<div class="card-text mb-2">${item.description}</div>` : ''}
                        <a href="${item.link}" target="_blank" class="btn btn-outline-primary btn-sm">Читать полностью</a>
                    </div>
                </div>
            `;
        });
        modalBody.innerHTML = html + '</div>';
    } catch (error) {
        modalBody.innerHTML = '<div class="alert alert-danger">Ошибка загрузки новостей.</div>';
    }
});