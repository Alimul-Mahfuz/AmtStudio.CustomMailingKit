namespace AmtStudio.CustomMailingKit.Models;

/// <summary>
/// Represents a file or inline content attached to an email.
/// </summary>
public sealed class EmailAttachment
{
    /// <summary>
    /// Gets the file name presented in the email.
    /// </summary>
    public string FileName { get; }

    /// <summary>
    /// Gets the MIME content type (e.g. "application/pdf", "image/png").
    /// </summary>
    public string? ContentType { get; }

    /// <summary>
    /// Gets the raw byte content if created from bytes.
    /// </summary>
    public byte[]? ContentBytes { get; }

    /// <summary>
    /// Gets the readable stream if created from a stream.
    /// </summary>
    public Stream? ContentStream { get; }

    /// <summary>
    /// Gets the physical file path if created from a local file.
    /// </summary>
    public string? FilePath { get; }

    /// <summary>
    /// Gets whether this attachment is an inline resource (e.g., embedded image).
    /// </summary>
    public bool IsInline { get; }

    /// <summary>
    /// Gets the Content-ID used for inline HTML referencing (e.g. &lt;img src="cid:logo"&gt;).
    /// </summary>
    public string? ContentId { get; }

    private EmailAttachment(
        string fileName,
        string? contentType,
        byte[]? contentBytes,
        Stream? contentStream,
        string? filePath,
        bool isInline,
        string? contentId)
    {
        FileName = fileName;
        ContentType = contentType;
        ContentBytes = contentBytes;
        ContentStream = contentStream;
        FilePath = filePath;
        IsInline = isInline;
        ContentId = contentId;
    }

    /// <summary>
    /// Creates an attachment from a file on disk.
    /// </summary>
    public static EmailAttachment FromFilePath(string filePath, string? contentType = null, string? fileName = null)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));

        var name = string.IsNullOrWhiteSpace(fileName) ? Path.GetFileName(filePath) : fileName;
        return new EmailAttachment(name, contentType, null, null, filePath, false, null);
    }

    /// <summary>
    /// Creates an attachment from a byte array.
    /// </summary>
    public static EmailAttachment FromBytes(byte[] content, string fileName, string? contentType = null)
    {
        ArgumentNullException.ThrowIfNull(content);
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name cannot be null or empty.", nameof(fileName));

        return new EmailAttachment(fileName, contentType, content, null, null, false, null);
    }

    /// <summary>
    /// Creates an attachment from a readable stream.
    /// </summary>
    public static EmailAttachment FromStream(Stream stream, string fileName, string? contentType = null)
    {
        ArgumentNullException.ThrowIfNull(stream);
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name cannot be null or empty.", nameof(fileName));

        return new EmailAttachment(fileName, contentType, null, stream, null, false, null);
    }

    /// <summary>
    /// Creates an inline linked resource (e.g. an image referenced in HTML with cid:contentId).
    /// </summary>
    public static EmailAttachment CreateInline(string contentId, byte[] content, string fileName, string? contentType = null)
    {
        ArgumentNullException.ThrowIfNull(content);
        if (string.IsNullOrWhiteSpace(contentId))
            throw new ArgumentException("ContentId cannot be null or empty.", nameof(contentId));
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name cannot be null or empty.", nameof(fileName));

        return new EmailAttachment(fileName, contentType, content, null, null, true, contentId);
    }

    /// <summary>
    /// Creates an inline linked resource from a stream.
    /// </summary>
    public static EmailAttachment CreateInline(string contentId, Stream stream, string fileName, string? contentType = null)
    {
        ArgumentNullException.ThrowIfNull(stream);
        if (string.IsNullOrWhiteSpace(contentId))
            throw new ArgumentException("ContentId cannot be null or empty.", nameof(contentId));
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name cannot be null or empty.", nameof(fileName));

        return new EmailAttachment(fileName, contentType, null, stream, null, true, contentId);
    }

    /// <summary>
    /// Creates an inline linked resource from a local file path.
    /// </summary>
    public static EmailAttachment CreateInlineFromFile(string contentId, string filePath, string? contentType = null)
    {
        if (string.IsNullOrWhiteSpace(contentId))
            throw new ArgumentException("ContentId cannot be null or empty.", nameof(contentId));
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));

        var fileName = Path.GetFileName(filePath);
        return new EmailAttachment(fileName, contentType, null, null, filePath, true, contentId);
    }
}
