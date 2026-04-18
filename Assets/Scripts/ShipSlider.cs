using UnityEngine;

public class ShipSlider : MonoBehaviour
{
	private Vector3 offset;
	private float zCoord;
	private Rigidbody rb;
	private bool hasWon = false;

	[Header("Movement Settings")]
	public bool positionHorizontal;
	public float gridStep = 1f;
	public bool isEvenLength;
	public bool is1x1; // Toggle for ships that move in all directions

	[Header("Win Condition")]
	public bool isPlayerShip;

	void Start()
	{
		// Get physics component and set to kinematic so it doesn't fall
		rb = GetComponent<Rigidbody>();
		rb.isKinematic = true;
	}

	private void OnCollisionEnter(Collision collision)
	{
		// Simple collision check for the goal
		if (isPlayerShip && collision.gameObject.CompareTag("Goal"))
		{
			WinLevel();
		}
	}

	void WinLevel()
	{
		// Stop logic if already won
		if (hasWon) return;
		hasWon = true;

		Debug.Log("Goal Reached!");
		// Visual feedback for win
		GetComponent<Renderer>().material.color = Color.green;

		// Find load next level from the GameController
		GameController controller = Object.FindFirstObjectByType<GameController>();
		if (controller != null)
		{
			controller.CompleteLevel();
		}
	}

	void OnMouseDown()
	{
		// Reset kinematic just in case and show active color
		rb.isKinematic = true;

		// Calculate depth and offset for dragging
		zCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
		offset = gameObject.transform.position - GetMouseWorldPos();
	}

	private Vector3 GetMouseWorldPos()
	{
		// Convert screen mouse position to world space
		Vector3 mousePoint = Input.mousePosition;
		mousePoint.z = zCoord;
		return Camera.main.ScreenToWorldPoint(mousePoint);
	}

	void OnMouseDrag()
	{
		Vector3 targetPos = GetMouseWorldPos() + offset;
		Vector3 currentPos = transform.position;
		Vector3 direction;

		// 1. Determine direction based on your constraints
		if (is1x1)
		{
			direction = targetPos - currentPos;
		}
		else if (positionHorizontal)
		{
			direction = new Vector3(targetPos.x - currentPos.x, 0, 0);
		}
		else
		{
			direction = new Vector3(0, 0, targetPos.z - currentPos.z);
		}

		float distance = direction.magnitude;

		// Only run if there is movement
		if (distance > 0.0001f)
		{
			RaycastHit hit;

			// Padding for ships to slide past eachother easier
			float castPadding = 0.02f;
			// Check if the path is blocked
			if (rb.SweepTest(direction.normalized, out hit, distance))
			{
				// Player exit logic
				if (isPlayerShip && hit.collider.CompareTag("Goal"))
				{
					// Move exactly to target since it's an exit
					ApplyPosition(targetPos);
					if (!hasWon) WinLevel();
				}
				// NPC exit logic
				else if (!isPlayerShip && hit.collider.CompareTag("NPCexit"))
				{
					ApplyPosition(targetPos);
				}
				else
				{
					// Hit the beam, move as far as allowed
					float allowedDistance = Mathf.Max(0, hit.distance - 0.01f);
					transform.position += direction.normalized * allowedDistance;
				}
			}
			else
			{
				// Path is clear and able to be moved
				ApplyPosition(targetPos);
			}
		}
	}

	// Helper to keep 1x1 vs Long ship constraints consistent
	void ApplyPosition(Vector3 target)
	{
		if (is1x1)
		{
			transform.position = target;
		}
		else if (positionHorizontal)
		{
			transform.position = new Vector3(target.x, transform.position.y, transform.position.z);
		}
		else
		{
			transform.position = new Vector3(transform.position.x, transform.position.y, target.z);
		}
	}

	void OnMouseUp()
	{
		// Default offsets for 1x1 or odd-length ships
		float offsetX = 0.5f;
		float offsetZ = 0.5f;

		// Handle snapping rules for different ship shapes
		if (is1x1)
		{
			// Always snap center for 1x1
			offsetX = 0.5f;
			offsetZ = 0.5f;
		}
		else if (!positionHorizontal)
		{
			// Adjust Z offset for even vertical ships
			if (isEvenLength) offsetZ = 0f;
		}
		else
		{
			// Adjust X offset for even horizontal ships
			if (isEvenLength) offsetX = 0f;
		}

		// Round position to nearest grid point
		float snappedX = Mathf.Round((transform.position.x - offsetX) / gridStep) * gridStep + offsetX;
		float snappedZ = Mathf.Round((transform.position.z - offsetZ) / gridStep) * gridStep + offsetZ;

		// Apply snapped position
		transform.position = new Vector3(snappedX, transform.position.y, snappedZ);
	}
}